using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using syswb_Taller.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers;

[Route("Vehiculo")]
public class VehiculoController : Controller
{
    private readonly BdTallerContext _bdTallerContext;

    public VehiculoController(BdTallerContext bdTallerContext)
    {
        _bdTallerContext = bdTallerContext;
    }

    // NOTA: Get_Vehiculos no devuelve idCliente, por eso idCliente solo se pasa a la vista
    // y no se usa como filtro.
    [HttpGet("")]
    [HttpGet("Lista")]
    [Authorize(Roles = RolesSistema.Todos)]
    public async Task<IActionResult> Lista(int? idCliente = null, string? matricula = null, string? marca = null, bool? activo = null)
    {
        var vehiculos = new List<dynamic>();

        await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync("Get_Vehiculos",
            ("@Matricula", matricula),
            ("@Marca", marca),
            ("@Activo", activo)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                vehiculos.Add(new
                {
                    IdVehiculo = reader.GetInt32(0),
                    vhMatricula = reader.GetNullableString(1) ?? string.Empty,
                    vhMarca = reader.GetNullableString(2) ?? string.Empty,
                    vhModelo = reader.GetNullableString(3) ?? string.Empty,
                    vhAño = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                    vhColor = reader.GetNullableString(5) ?? string.Empty,
                    vhActivo = !reader.IsDBNull(6) && reader.GetBoolean(6)
                });
            }
        }

        ViewBag.IdCliente = idCliente;
        ViewBag.FiltroMatricula = matricula;
        ViewBag.FiltroMarca = marca;
        ViewBag.FiltroActivo = activo;

        return View(vehiculos);
    }

    // Selección de cliente para crear vehículo
    [HttpGet("SeleccionarCliente")]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> SeleccionarCliente(string? nombre = null, bool? activo = null)
    {
        var clientes = new List<dynamic>();

        await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync(
            "Get_Clientes", ("@Nombre", nombre), ("@Activo", activo)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                clientes.Add(new
                {
                    IdCliente = reader.GetInt32(0),
                    Nombre = reader.GetNullableString(1) ?? string.Empty,
                    Apellido = reader.GetNullableString(2) ?? string.Empty,
                    Telefono = reader.GetNullableString(3) ?? string.Empty
                });
            }
        }

        ViewBag.FiltroNombre = nombre;
        ViewBag.FiltroActivo = activo;

        return View(clientes);
    }

    // Formulario de creación de vehículo para un cliente dado
    [HttpGet("Crear")]
    public async Task<IActionResult> Crear(int? idCliente, string? redirigirA = null)
    {
        if (!idCliente.HasValue)
        {
            TempData["Error"] = "Por favor selecciona primero un Cliente.";
            return RedirectToAction(nameof(SeleccionarCliente));
        }

        ViewBag.IdCliente = idCliente.Value;
        ViewBag.Redireccion = redirigirA;
        ViewBag.Productos = await _bdTallerContext.ProductoRepuestos.Where(p => p.PrActivo).AsNoTracking().ToListAsync();
        ViewBag.Servicios = await _bdTallerContext.Servicios.Where(s => s.SerActivo).AsNoTracking().ToListAsync();
        ViewBag.Detalles = new List<dynamic>();
        ViewBag.CostoTotal = 0.0;

        return View();
    }

    [HttpPost("Crear")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Crear(
        string vhMatricula,
        string vhMarca,
        string vhModelo,
        int vhAño,
        string vhColor,
        string vhEstado,
        int idCliente,
        string? redirigirA = null)
    {
        await using var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync("Ins_Vehiculo",
            ("@vhMatricula", vhMatricula),
            ("@vhMarca", vhMarca),
            ("@vhModelo", vhModelo),
            ("@vhAño", vhAño),
            ("@vhColor", vhColor),
            ("@vhEstado", vhEstado),
            ("@idCliente", idCliente));
        var outIdVehiculo = cmd.AddOutputParameter("@idVehiculo", SqlDbType.Int);

        await cmd.ExecuteNonQueryAsync();
        int idVehiculo = outIdVehiculo.Value is int outId ? outId : (int.TryParse(outIdVehiculo.Value?.ToString(), out var p) ? p : 0);

        return redirigirA switch
        {
            "Orden" => RedirectToAction(nameof(OrdenController.Crear), "Orden", new { clienteID = idCliente, vehiculoID = idVehiculo }),
            "Presupuesto" => RedirectToAction(nameof(PresupuestoController.CrearPresupuesto), "Presupuesto", new { clienteID = idCliente, idVehiculo }),
            _ => RedirectToAction(nameof(Lista))
        };
    }

    [HttpGet("Editar/{id?}")]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Editar(int? id, [FromQuery(Name = "id")] int? queryId)
    {
        int idFinal = id ?? queryId ?? 0;
        if (idFinal <= 0)
            return NotFound();

        const string sql = """
            SELECT vhMatricula, vhMarca, vhModelo, vhAño, vhColor, vhEstado, idCliente
              FROM Vehiculo
             WHERE idVehiculo = @idVehiculo
            """;

        dynamic? vehiculo = null;
        await using (var cmd = await _bdTallerContext.Database.CreateSqlCommandAsync(sql, ("@idVehiculo", idFinal)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            if (await reader.ReadAsync())
            {
                vehiculo = new
                {
                    IdVehiculo = idFinal,
                    Matricula = reader.GetNullableString(0) ?? string.Empty,
                    Marca = reader.GetNullableString(1) ?? string.Empty,
                    Modelo = reader.GetNullableString(2) ?? string.Empty,
                    Año = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                    Color = reader.GetNullableString(4) ?? string.Empty,
                    Estado = reader.GetNullableString(5) ?? string.Empty,
                    IdCliente = reader.IsDBNull(6) ? 0 : reader.GetInt32(6)
                };
            }
        }

        if (vehiculo == null)
            return NotFound();

        return View(vehiculo);
    }

    [HttpPost("Editar")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Editar(
        int idVehiculo,
        string vhMatricula,
        string vhMarca,
        string vhModelo,
        int vhAño,
        string vhColor,
        string vhEstado,
        int idCliente)
    {
        await _bdTallerContext.Database.ExecuteStoredProcedureAsync("Upd_Vehiculo",
            ("@idVehiculo", idVehiculo),
            ("@vhMatricula", vhMatricula),
            ("@vhMarca", vhMarca),
            ("@vhModelo", vhModelo),
            ("@vhAño", vhAño),
            ("@vhColor", vhColor),
            ("@vhEstado", vhEstado),
            ("@idCliente", idCliente));

        return RedirectToAction(nameof(Lista));
    }

    [HttpPost("CambiarEstado")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.AdminSupervisor)]
    public async Task<IActionResult> CambiarEstado(int idVehiculo)
    {
        await _bdTallerContext.Database.ExecuteStoredProcedureAsync("Mod_EstadoVh", ("@idVehiculo", idVehiculo));
        return RedirectToAction(nameof(Lista));
    }
}
