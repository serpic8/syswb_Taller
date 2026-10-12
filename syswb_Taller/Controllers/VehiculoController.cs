using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using syswb_Taller.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers;

public class VehiculoController : Controller
{
    private readonly BdTallerContext _bdTallerContext;

    public VehiculoController(BdTallerContext bdTallerContext)
    {
        _bdTallerContext = bdTallerContext;
    }

    // NOTA: Get_Vehiculos no devuelve idCliente, por eso idCliente solo se pasa a la vista
    // y no se usa como filtro.
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
                    vhMatricula = reader.GetString(1),
                    vhMarca = reader.GetString(2),
                    vhModelo = reader.GetString(3),
                    vhAño = reader.GetInt32(4),
                    vhColor = reader.GetString(5),
                    vhActivo = reader.GetBoolean(6)
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
                    Nombre = reader.GetString(1),
                    Apellido = reader.GetString(2),
                    Telefono = reader.GetString(3)
                });
            }
        }

        ViewBag.FiltroNombre = nombre;
        ViewBag.FiltroActivo = activo;

        return View(clientes);
    }

    // Formulario de creación de vehículo para un cliente dado
    [HttpGet("Crear")]
    [Authorize(Roles = RolesSistema.Gestion)]
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
        int idVehiculo = (int)outIdVehiculo.Value;

        return redirigirA switch
        {
            "Orden" => RedirectToAction(nameof(OrdenController.Crear), "Orden", new { clienteID = idCliente, vehiculoID = idVehiculo }),
            "Presupuesto" => RedirectToAction(nameof(PresupuestoController.CrearPresupuesto), "Presupuesto", new { clienteID = idCliente, idVehiculo }),
            _ => RedirectToAction(nameof(Lista))
        };
    }

    [HttpGet("Editar")]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Editar(int id)
    {
        const string sql = """
            SELECT vhMatricula, vhMarca, vhModelo, vhAño, vhColor, vhEstado, idCliente
              FROM Vehiculo
             WHERE idVehiculo = @idVehiculo
            """;

        dynamic? vehiculo = null;
        await using (var cmd = await _bdTallerContext.Database.CreateSqlCommandAsync(sql, ("@idVehiculo", id)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            if (await reader.ReadAsync())
            {
                vehiculo = new
                {
                    IdVehiculo = id,
                    Matricula = reader.GetString(0),
                    Marca = reader.GetString(1),
                    Modelo = reader.GetString(2),
                    Año = reader.GetInt32(3),
                    Color = reader.GetString(4),
                    Estado = reader.GetString(5),
                    IdCliente = reader.GetInt32(6)
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
