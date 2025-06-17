using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers
{
    public class CajaDiariaController : Controller
    {
        private readonly BdTallerContext _bdTallerContext;

        public CajaDiariaController(BdTallerContext bdTallerContext)
        {
            _bdTallerContext = bdTallerContext;
        }

        // POST: Abrir Caja
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AbrirCaja(int? anio, int? mes)
        {
            // Verificar si ya existe una caja para hoy
            bool yaExiste = await _bdTallerContext.CajaDiaria
                .AnyAsync(x => x.CjFecha == DateOnly.FromDateTime(DateTime.Today));

            if (yaExiste)
            {
                TempData["SuccessMessage"] = "⚠️ Ya existe una caja para hoy.";
            }
            else
            {
                await _bdTallerContext.Database.ExecuteSqlRawAsync("EXEC dbo.AbrirCaja");
                TempData["SuccessMessage"] = "✅ Caja abierta correctamente.";
            }

            int y = anio ?? DateTime.Today.Year;
            int m = mes ?? DateTime.Today.Month;

            return RedirectToAction(nameof(Lista), new { anio = y, mes = m });
        }

        // POST: Cerrar Caja
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarCaja(int? anio, int? mes)
        {
            await _bdTallerContext.Database.ExecuteSqlRawAsync("EXEC dbo.CerrarCaja");

            TempData["SuccessMessage"] = "✅ Caja cerrada correctamente.";

            int y = anio ?? DateTime.Today.Year;
            int m = mes ?? DateTime.Today.Month;

            return RedirectToAction(nameof(Lista), new { anio = y, mes = m });
        }

        // GET: Resumen de Caja Diaria
        public async Task<IActionResult> Lista(int? anio, int? mes)
        {
            int y = anio ?? DateTime.Today.Year;
            int m = mes ?? DateTime.Today.Month;

            var resumen = new List<dynamic>();
            var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "dbo.Get_CajaDiariaResumen";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Anio", y));
                cmd.Parameters.Add(new SqlParameter("@Mes", m));

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    resumen.Add(new
                    {
                        Fecha = reader.GetDateTime(0).Date,
                        Ingreso = reader.GetDouble(1),
                        Egreso = reader.GetDouble(2),
                        Balance = reader.GetDouble(3)
                    });
                }
            }

            // Verifica si hay una caja abierta
            bool cajaAbierta = await _bdTallerContext.CajaDiaria
                .AsNoTracking()
                .AnyAsync(c => c.CjCierre == null);

            ViewBag.Resumen = resumen;
            ViewBag.SelectedYear = y;
            ViewBag.SelectedMonth = m;
            ViewBag.CajaAbierta = cajaAbierta;

            return View();
        }

        // GET: Detalle de Caja Diaria para una fecha específica
        // GET: Detalle de Caja Diaria para una caja específica
        public async Task<IActionResult> Detalle(int idCaja)
        {
            var caja = await _bdTallerContext.CajaDiaria
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdCaja == idCaja);

            if (caja == null)
            {
                // No existe la caja con ese Id
                ViewBag.IngresosEgresos = new List<dynamic>();
                ViewBag.DetalleFinanzas = new List<dynamic>();
                ViewBag.Fecha = null;
                ViewBag.IdCaja = null;
                return View();
            }

            DateTime fechaCaja = caja.CjFecha.ToDateTime(TimeOnly.MinValue);

            var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            var ingresosEgresos = new List<dynamic>();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "dbo.Get_CajaDiariaIngresosEgresosPorDia";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdCaja", idCaja));

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    ingresosEgresos.Add(new
                    {
                        Fecha = reader.GetDateTime(0).Date,
                        TotalIngresos = reader.GetDouble(1),
                        TotalEgresos = reader.GetDouble(2),
                        Balance = reader.GetDouble(3)
                    });
                }
            }

            var detalleFinanzas = new List<dynamic>();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "dbo.Get_CajaDiariaDetalleFinanzas";
                cmd.CommandType = CommandType.StoredProcedure;

                var pFecha = cmd.CreateParameter();
                pFecha.ParameterName = "@Fecha";
                pFecha.DbType = DbType.Date;
                pFecha.Value = fechaCaja.Date;
                cmd.Parameters.Add(pFecha);

                cmd.Parameters.Add(new SqlParameter("@IdCaja", idCaja));

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    detalleFinanzas.Add(new
                    {
                        FechaHora = reader.GetDateTime(0),
                        TipoMovimiento = reader.GetString(1),
                        Responsable = reader.IsDBNull(2) ? "" : reader.GetString(2),
                        Monto = reader.GetDouble(3)
                    });
                }
            }

            ViewBag.Fecha = fechaCaja;
            ViewBag.IngresosEgresos = ingresosEgresos;
            ViewBag.DetalleFinanzas = detalleFinanzas;
            ViewBag.IdCaja = idCaja;

            return View();
        }

    }
}
