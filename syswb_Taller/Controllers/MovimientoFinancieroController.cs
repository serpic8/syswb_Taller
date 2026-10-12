using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using syswb_Taller.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers;

public class MovimientoFinancieroController : Controller
{
    private readonly BdTallerContext _bdTallerContext;

    public MovimientoFinancieroController(BdTallerContext bdTallerContext)
    {
        _bdTallerContext = bdTallerContext;
    }

    [Authorize(Roles = RolesSistema.Administrador)]
    public async Task<IActionResult> Resumen(int? anio, int? mes)
    {
        int y = anio ?? DateTime.Today.Year;
        int m = mes ?? DateTime.Today.Month;

        // El SP devuelve tres result sets en una sola ejecución
        await using var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync(
            "dbo.Get_ResumenMes", ("@Anio", y), ("@Mes", m));
        await using var reader = await cmd.ExecuteReaderAsync();

        // 1) Totales
        ViewBag.TotalIngresos = 0.0;
        ViewBag.TotalEgresos = 0.0;
        ViewBag.Balance = 0.0;
        if (await reader.ReadAsync())
        {
            ViewBag.TotalIngresos = reader.GetNullableDouble(0) ?? 0.0;
            ViewBag.TotalEgresos = reader.GetNullableDouble(1) ?? 0.0;
            ViewBag.Balance = reader.GetNullableDouble(2) ?? 0.0;
        }

        // 2) Actividades
        await reader.NextResultAsync();
        var actividades = new List<MovimientoFinanciero>();
        while (await reader.ReadAsync())
        {
            DateTime fechaMov = reader.GetNullableDateTime(0)
                ?? (DateTime.TryParse(reader.GetValue(0)?.ToString(), out var dtParsed) ? dtParsed : DateTime.Now);

            actividades.Add(new MovimientoFinanciero
            {
                MovFecha = fechaMov,
                MvTipoMovimiento = reader.GetNullableString(1) ?? string.Empty,
                MovMonto = reader.GetNullableDouble(2) ?? 0.0,
                IdPagoOrden = reader.GetNullableInt32(3),
                IdCompra = reader.GetNullableInt32(4),
                IdCaja = reader.GetNullableInt32(5) ?? 0
            });
        }

        // 3) Órdenes junto con su pago
        await reader.NextResultAsync();
        var ordenesConPago = new List<dynamic>();
        while (await reader.ReadAsync())
        {
            ordenesConPago.Add(new
            {
                IdOrden = reader.GetInt32(0),
                OrFechaCreacion = reader.GetNullableDateTime(1),
                OrEstado = reader.GetNullableString(2) ?? string.Empty,
                FechaPago = reader.GetNullableDateTime(3),
                MontoPagado = reader.GetNullableDouble(4) ?? 0.0
            });
        }

        ViewBag.Actividades = actividades;
        ViewBag.Ordenes = ordenesConPago;
        ViewBag.SelectedYear = y;
        ViewBag.SelectedMonth = m;
        return View();
    }

    [HttpGet]
    [Authorize(Roles = RolesSistema.AdminSupervisor)]
    public async Task<IActionResult> ListaMovimientos(
        int? anio,
        int? mes,
        int? dia,
        string tipoMovimiento,
        int pageNumber = 1,
        int pageSize = 20)
    {
        var movimientos = new List<dynamic>();
        int totalRegistros = 0;

        await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync(
            "dbo.Get_MovimientosFinancieros",
            ("@Anio", anio),
            ("@Mes", mes),
            ("@Dia", dia),
            ("@TipoMovimiento", string.IsNullOrWhiteSpace(tipoMovimiento) ? null : tipoMovimiento),
            ("@PageNumber", pageNumber),
            ("@PageSize", pageSize)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            // 1) Movimientos paginados
            while (await reader.ReadAsync())
            {
                movimientos.Add(new
                {
                    IdMovimiento = reader.GetInt32(0),
                    MovFecha = reader.GetNullableDateTime(1) ?? DateTime.Now,
                    FechaCorta = reader.GetNullableString(2) ?? string.Empty,
                    Hora = reader.GetNullableString(3) ?? string.Empty,
                    TipoMovimiento = reader.GetNullableString(4) ?? string.Empty,
                    Responsable = reader.GetNullableString(5) ?? string.Empty,
                    MetodoPago = reader.GetNullableString(6) ?? "N/A",
                    Monto = reader.GetNullableDouble(7) ?? 0.0
                });
            }

            // 2) Total de registros
            if (await reader.NextResultAsync() && await reader.ReadAsync())
                totalRegistros = reader.GetInt32(0);
        }

        ViewBag.Movimientos = movimientos;
        ViewBag.Anio = anio;
        ViewBag.Mes = mes;
        ViewBag.Dia = dia;
        ViewBag.TipoMovimiento = tipoMovimiento;
        ViewBag.PageNumber = pageNumber;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalRegistros / pageSize);

        return View();
    }
}
