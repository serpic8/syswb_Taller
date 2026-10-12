using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using syswb_Taller.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers;

public class ServicioController : Controller
{
    private readonly BdTallerContext _bdTallerContext;

    public ServicioController(BdTallerContext bdTallerContext)
    {
        _bdTallerContext = bdTallerContext;
    }

    [HttpGet]
    [Authorize(Roles = RolesSistema.Todos)]
    public async Task<IActionResult> Lista(string? nombre = null, bool? activo = null)
    {
        var servicios = new List<Servicio>();

        await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync(
            "Get_Servicios", ("@Nombre", nombre), ("@Activo", activo)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                servicios.Add(new Servicio
                {
                    IdServicio = reader.GetInt32(0),
                    SerNombre = reader.GetNullableString(1) ?? string.Empty,
                    SerPrecioUnit = reader.GetNullableDouble(2) ?? 0.0,
                    SerActivo = !reader.IsDBNull(3) && reader.GetBoolean(3)
                });
            }
        }

        ViewBag.FiltroNombre = nombre;
        ViewBag.FiltroActivo = activo;

        return View(servicios);
    }

    [HttpGet]
    [Authorize(Roles = RolesSistema.Gestion)]
    public IActionResult Crear() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Crear(string serNombre, double serPrecioUnit)
    {
        await _bdTallerContext.Database.ExecuteStoredProcedureAsync("Ins_Servicio",
            ("@serNombre", serNombre),
            ("@serPrecioUnit", serPrecioUnit));

        return RedirectToAction(nameof(Lista));
    }

    [HttpGet]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Editar(int id)
    {
        dynamic? servicio = null;

        await using (var cmd = await _bdTallerContext.Database.CreateSqlCommandAsync(
            "SELECT idServicio, serNombre, serPrecioUnit FROM Servicio WHERE idServicio = @id",
            ("@id", id)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            if (await reader.ReadAsync())
            {
                servicio = new
                {
                    IdServicio = reader.GetInt32(0),
                    SerNombre = reader.GetNullableString(1) ?? string.Empty,
                    SerPrecioUnit = reader.GetNullableDouble(2) ?? 0.0
                };
            }
        }

        if (servicio == null)
            return NotFound();

        ViewBag.Servicio = servicio;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Editar(int idServicio, string serNombre, double serPrecioUnit)
    {
        await _bdTallerContext.Database.ExecuteStoredProcedureAsync("Upd_Servicio",
            ("@idServicio", idServicio),
            ("@serNombre", serNombre),
            ("@serPrecioUnit", serPrecioUnit));

        return RedirectToAction(nameof(Lista));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.AdminSupervisor)]
    public async Task<IActionResult> CambiarEstado(int idServicio)
    {
        await _bdTallerContext.Database.ExecuteStoredProcedureAsync("Mod_EstadoSer",
            ("@idServicio", idServicio));

        return RedirectToAction(nameof(Lista));
    }
}
