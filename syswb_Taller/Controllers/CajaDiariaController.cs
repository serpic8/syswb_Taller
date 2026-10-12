using Microsoft.AspNetCore.Authorization;
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

        [HttpPost]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AbrirCaja(int? anio, int? mes)
        {
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

        [HttpPost]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarCaja(int? anio, int? mes)
        {
            await _bdTallerContext.Database.ExecuteSqlRawAsync("EXEC dbo.CerrarCaja");

            TempData["SuccessMessage"] = "✅ Caja cerrada correctamente.";

            int y = anio ?? DateTime.Today.Year;
            int m = mes ?? DateTime.Today.Month;

            return RedirectToAction(nameof(Lista), new { anio = y, mes = m });
        }

        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> Lista(int? anio, int? mes)
        {
            int y = anio ?? DateTime.Today.Year;
            int m = mes ?? DateTime.Today.Month;

            var resumen = new List<dynamic>();
            using var conn = _bdTallerContext.Database.GetDbConnection();
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
                        IdCaja = reader.GetInt32(0),        
                        Fecha = reader.GetDateTime(1).Date,
                        Ingreso = reader.GetDouble(2),
                        Egreso = reader.GetDouble(3),
                        Balance = reader.GetDouble(4)
                    });
                }
            }

            bool cajaAbierta = await _bdTallerContext.CajaDiaria
                .AsNoTracking()
                .AnyAsync(c => c.CjCierre == null);

            ViewBag.Resumen = resumen;
            ViewBag.SelectedYear = y;
            ViewBag.SelectedMonth = m;
            ViewBag.CajaAbierta = cajaAbierta;

            return View();
        }



        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        [HttpGet]
        public async Task<IActionResult> Detalle(int idCaja, DateTime fecha)
        {
            var dtDetail = new DataTable();
            var dtResumen = new DataTable();

            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Get_CajaDiariaDetalleFinanzas";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Fecha", SqlDbType.Date) { Value = fecha.Date });
                cmd.Parameters.Add(new SqlParameter("@IdCaja", SqlDbType.Int) { Value = idCaja });

                using var reader = await cmd.ExecuteReaderAsync();
                dtDetail.Load(reader);
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Get_CajaDiariaIngresosEgresosPorDia";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdCaja", SqlDbType.Int) { Value = idCaja });

                using var reader = await cmd.ExecuteReaderAsync();
                dtResumen.Load(reader);
            }

            ViewBag.Detail = dtDetail;
            ViewBag.Resumen = dtResumen;
            ViewBag.IdCaja = idCaja;
            ViewBag.Fecha = fecha.ToString("yyyy-MM-dd");

            return View();
        }

    }
}
