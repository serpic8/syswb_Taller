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
        if (await reader.ReadAsync())
        {
            ViewBag.TotalIngresos = reader.GetDouble(0);
            ViewBag.TotalEgresos = reader.GetDouble(1);
            ViewBag.Balance = reader.GetDouble(2);
        }

        // 2) Actividades
        await reader.NextResultAsync();
        var actividades = new List<MovimientoFinanciero>();
        while (await reader.ReadAsync())
        {
            actividades.Add(new MovimientoFinanciero
            {
                MovFecha = DateTime.Parse(reader.GetString(0)),
                MvTipoMovimiento = reader.GetString(1),
                MovMonto = reader.GetDouble(2),
                IdPagoOrden = reader.GetNullableInt32(3),
                IdCompra = reader.GetNullableInt32(4),
                IdCaja = reader.GetInt32(5)
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
                OrFechaCreacion = reader.GetDateTime(1),
                OrEstado = reader.GetString(2),
                FechaPago = reader.GetDateTime(3),
                MontoPagado = reader.GetDouble(4)
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
                    MovFecha = reader.GetDateTime(1),
                    FechaCorta = reader.GetString(2),
                    Hora = reader.GetString(3),
                    TipoMovimiento = reader.GetString(4),
                    Responsable = reader.GetString(5),
                    MetodoPago = reader.GetNullableString(6) ?? "N/A",
                    Monto = reader.GetDouble(7)
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
