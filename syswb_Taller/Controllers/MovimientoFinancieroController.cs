using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers
{
    public class MovimientoFinancieroController : Controller
    {
        private readonly BdTallerContext _bdTallerContext;

        public MovimientoFinancieroController(BdTallerContext bdTallerContext)
        {
            _bdTallerContext = bdTallerContext;
        }

        [Authorize(Roles ="Administrador")]
        public async Task<IActionResult> Resumen(int? anio, int? mes)
        {
            int y = anio ?? DateTime.Today.Year;
            int m = mes ?? DateTime.Today.Month;

            // 1) Abre la conexión del DbContext
            var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            // 2) Ejecuta el SP una única vez
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "dbo.Get_ResumenMes";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            var pAnio = cmd.CreateParameter();
            pAnio.ParameterName = "@Anio";
            pAnio.Value = y;
            cmd.Parameters.Add(pAnio);

            var pMes = cmd.CreateParameter();
            pMes.ParameterName = "@Mes";
            pMes.Value = m;
            cmd.Parameters.Add(pMes);

            using var reader = await cmd.ExecuteReaderAsync();

            // — Result set 1: Totales —
            if (await reader.ReadAsync())
            {
                ViewBag.TotalIngresos = reader.GetDouble(0);
                ViewBag.TotalEgresos = reader.GetDouble(1);
                ViewBag.Balance = reader.GetDouble(2);
            }

            // — Result set 2: Actividades —
            await reader.NextResultAsync();
            var actividades = new List<MovimientoFinanciero>();
            while (await reader.ReadAsync())
            {
                actividades.Add(new MovimientoFinanciero
                {
                    MovFecha = DateTime.Parse(reader.GetString(0)),
                    MvTipoMovimiento = reader.GetString(1),
                    MovMonto = reader.GetDouble(2),
                    IdPagoOrden = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                    IdCompra = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                    IdCaja = reader.GetInt32(5)
                });
            }
            ViewBag.Actividades = actividades;

            // — Result set 3: Órdenes junto con su pago —
            await reader.NextResultAsync();
            var ordenesConPago = new List<dynamic>();
            while (await reader.ReadAsync())
            {
                ordenesConPago.Add(new
                {
                    IdOrden = reader.GetInt32(0),
                    OrFechaCreacion = reader.GetDateTime(1),
                    OrEstado = reader.GetString(2),
                    FechaPago = reader.GetDateTime(3),
                    MontoPagado = reader.GetDouble(4)
                });
            }
            ViewBag.Ordenes = ordenesConPago;
            ViewBag.SelectedYear = y;
            ViewBag.SelectedMonth = m;
            return View();
        }

        [Authorize(Roles = "Administrador, Supervisor")]
        public async Task<IActionResult> ListaMovimientos(
         int? anio,
         int? mes,
         int? dia,
         string tipoMovimiento,
         int pageNumber = 1,
         int pageSize = 20) // ← Mostrará 20 por página
        {
            // 1) Abrir conexión manualmente
            var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "dbo.Get_MovimientosFinancieros";
            cmd.CommandType = CommandType.StoredProcedure;

            // 2) Parámetros del SP
            cmd.Parameters.Add(new SqlParameter("@Anio", anio ?? (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Mes", mes ?? (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Dia", dia ?? (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@TipoMovimiento", string.IsNullOrEmpty(tipoMovimiento) ? (object)DBNull.Value : tipoMovimiento));
            cmd.Parameters.Add(new SqlParameter("@PageNumber", pageNumber));
            cmd.Parameters.Add(new SqlParameter("@PageSize", pageSize));

            var movimientos = new List<dynamic>();

            using var reader = await cmd.ExecuteReaderAsync();

            // Primer result set: movimientos paginados
            while (await reader.ReadAsync())
            {
                movimientos.Add(new
                {
                    IdMovimiento = reader.GetInt32(0),
                    MovFecha = reader.GetDateTime(1),
                    FechaCorta = reader.GetString(2),
                    Hora = reader.GetString(3),
                    TipoMovimiento = reader.GetString(4),
                    Responsable = reader.GetString(5),
                    MetodoPago = reader.IsDBNull(6) ? "N/A" : reader.GetString(6),
                    Monto = reader.GetDouble(7)
                });
            }

            // Segundo result set: total de registros
            int totalRegistros = 0;
            int totalPages = 1;

            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                totalRegistros = reader.GetInt32(0);
                totalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);
            }

            // 3) Pasar datos a la Vista
            ViewBag.Movimientos = movimientos;
            ViewBag.Anio = anio;
            ViewBag.Mes = mes;
            ViewBag.Dia = dia;
            ViewBag.TipoMovimiento = tipoMovimiento;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;

            return View();
        }



    }
}
