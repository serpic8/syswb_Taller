using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using syswb_Taller.Data;
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
        [Authorize(Roles = RolesSistema.Gestion)]
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
        [Authorize(Roles = RolesSistema.Gestion)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarCaja(int? anio, int? mes)
        {
            await _bdTallerContext.Database.ExecuteSqlRawAsync("EXEC dbo.CerrarCaja");

            TempData["SuccessMessage"] = "✅ Caja cerrada correctamente.";

            int y = anio ?? DateTime.Today.Year;
            int m = mes ?? DateTime.Today.Month;

            return RedirectToAction(nameof(Lista), new { anio = y, mes = m });
        }

        [Authorize(Roles = RolesSistema.Gestion)]
        public async Task<IActionResult> Lista(int? anio, int? mes)
        {
            int y = anio ?? DateTime.Today.Year;
            int m = mes ?? DateTime.Today.Month;

            var resumen = new List<dynamic>();
            await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync(
                "dbo.Get_CajaDiariaResumen", ("@Anio", y), ("@Mes", m)))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    resumen.Add(new
                    {
                        IdCaja = reader.GetInt32(0),        
                        Fecha = reader.GetNullableDateTime(1)?.Date ?? DateTime.Today,
                        Ingreso = reader.GetNullableDouble(2) ?? 0.0,
                        Egreso = reader.GetNullableDouble(3) ?? 0.0,
                        Balance = reader.GetNullableDouble(4) ?? 0.0
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

        [Authorize(Roles = RolesSistema.Gestion)]
        [HttpGet]
        public async Task<IActionResult> Detalle(int idCaja, DateTime fecha)
        {
            var dtDetail = new DataTable();
            var dtResumen = new DataTable();

            await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync(
                "dbo.Get_CajaDiariaDetalleFinanzas",
                ("@Fecha", fecha.Date),
                ("@IdCaja", idCaja)))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                dtDetail.Load(reader);
            }

            await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync(
                "dbo.Get_CajaDiariaIngresosEgresosPorDia",
                ("@IdCaja", idCaja)))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
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
