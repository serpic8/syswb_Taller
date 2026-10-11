using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using syswb_Taller.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers;

[Authorize(Roles = RolesSistema.Gestion)]
public class CajaDiariaController : Controller
{
    private readonly BdTallerContext _bdTallerContext;

    public CajaDiariaController(BdTallerContext bdTallerContext)
    {
        _bdTallerContext = bdTallerContext;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AbrirCaja(int? anio, int? mes)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        bool yaExiste = await _bdTallerContext.CajaDiaria.AnyAsync(c => c.CjFecha == hoy);

        if (yaExiste)
        {
            TempData["SuccessMessage"] = "⚠️ Ya existe una caja para hoy.";
        }
        else
        {
            await _bdTallerContext.Database.ExecuteSqlRawAsync("EXEC dbo.AbrirCaja");
            TempData["SuccessMessage"] = "✅ Caja abierta correctamente.";
        }

        return RedirigirALista(anio, mes);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CerrarCaja(int? anio, int? mes)
    {
        await _bdTallerContext.Database.ExecuteSqlRawAsync("EXEC dbo.CerrarCaja");
        TempData["SuccessMessage"] = "✅ Caja cerrada correctamente.";

        return RedirigirALista(anio, mes);
    }

    // GET: Resumen de Caja Diaria
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
                    Fecha = reader.GetDateTime(1).Date,
                    Ingreso = reader.GetDouble(2),
                    Egreso = reader.GetDouble(3),
                    Balance = reader.GetDouble(4)
                });
            }
        }

        ViewBag.Resumen = resumen;
        ViewBag.SelectedYear = y;
        ViewBag.SelectedMonth = m;
        ViewBag.CajaAbierta = await _bdTallerContext.CajaDiaria
            .AsNoTracking()
            .AnyAsync(c => c.CjCierre == null);

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int idCaja, DateTime fecha)
    {
        // 1) Detalle de movimientos para el día
        var dtDetalle = new DataTable();
        await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync(
            "Get_CajaDiariaDetalleFinanzas", ("@Fecha", fecha.Date), ("@IdCaja", idCaja)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            dtDetalle.Load(reader);
        }

        // 2) Resumen de ingresos/egresos de esa caja
        var dtResumen = new DataTable();
        await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync(
            "Get_CajaDiariaIngresosEgresosPorDia", ("@IdCaja", idCaja)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            dtResumen.Load(reader);
        }

        ViewBag.Detail = dtDetalle;
        ViewBag.Resumen = dtResumen;
        ViewBag.IdCaja = idCaja;
        ViewBag.Fecha = fecha.ToString("yyyy-MM-dd");

        return View();
    }

    private RedirectToActionResult RedirigirALista(int? anio, int? mes) =>
        RedirectToAction(nameof(Lista), new
        {
            anio = anio ?? DateTime.Today.Year,
            mes = mes ?? DateTime.Today.Month
        });
}
