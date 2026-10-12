using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using syswb_Taller.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers;

[Route("Cliente")]
public class ClienteController : Controller
{
    private readonly BdTallerContext _bdTallerContext;

    public ClienteController(BdTallerContext bdTallerContext)
    {
        _bdTallerContext = bdTallerContext;
    }

    // GET: /Cliente/Lista
    [Authorize(Roles = RolesSistema.Gestion)]
    [HttpGet("Lista")]
    public async Task<IActionResult> Lista(string? nombre = null, bool? activo = null)
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
                    Telefono = reader.GetNullableString(3) ?? string.Empty,
                    OrdenesRealizadas = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                    Activo = !reader.IsDBNull(5) && reader.GetBoolean(5)
                });
            }
        }

        ViewBag.FiltroNombre = nombre;
        ViewBag.FiltroActivo = activo;
        return View(clientes);
    }

    [Authorize(Roles = RolesSistema.Gestion)]
    [HttpGet("Crear")]
    public IActionResult Crear(string? redirigirA = null)
    {
        ViewBag.Redireccion = redirigirA;
        return View();
    }

    [Authorize(Roles = RolesSistema.Gestion)]
    [HttpPost("Crear")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        string pNombre,
        string pApellido,
        string pTelefono,
        string pCedula,
        string clEmail,
        string? redirigirA = null)
    {
        await using var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync("Ins_Cliente",
            ("@pNombre", pNombre),
            ("@pApellido", pApellido),
            ("@pTelefono", pTelefono?.Replace("-", "") ?? string.Empty),
            ("@pCedula", pCedula),
            ("@clEmail", clEmail));
        var outIdCliente = cmd.AddOutputParameter("@OutIdCliente", SqlDbType.Int);

        await cmd.ExecuteNonQueryAsync();
        int idCliente = outIdCliente.Value is int outId ? outId : (int.TryParse(outIdCliente.Value?.ToString(), out var p) ? p : 0);

        if (!string.IsNullOrEmpty(redirigirA))
            return RedirectToAction(nameof(VehiculoController.Crear), "Vehiculo", new { idCliente, redirigirA });

        return RedirectToAction(nameof(Lista));
    }

    [Authorize(Roles = RolesSistema.Gestion)]
    [HttpGet("Editar/{idCliente?}")]
    public async Task<IActionResult> Editar(int? idCliente, [FromQuery(Name = "id")] int? queryId)
    {
        int idFinal = idCliente ?? queryId ?? 0;
        if (idFinal <= 0)
            return NotFound();

        const string sqlCliente = """
            SELECT c.idCliente,
                   p.pNombre,
                   p.pApellido,
                   p.pTelefono,
                   p.pCedula,
                   c.clEmail
              FROM Cliente c
              JOIN Persona p ON p.idPersona = c.idPersona
             WHERE c.idCliente = @idCliente
            """;

        dynamic? cliente = null;
        await using (var cmd = await _bdTallerContext.Database.CreateSqlCommandAsync(sqlCliente, ("@idCliente", idFinal)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            if (await reader.ReadAsync())
            {
                cliente = new
                {
                    IdCliente = reader.GetInt32(0),
                    Nombre = reader.GetNullableString(1),
                    Apellido = reader.GetNullableString(2),
                    Telefono = reader.GetNullableString(3),
                    Cedula = reader.GetNullableString(4),
                    Email = reader.GetNullableString(5)
                };
            }
        }

        if (cliente == null)
            return NotFound();

        var vehiculos = new List<dynamic>();
        await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync(
            "Get_VehiculosPorCliente", ("@idCliente", idFinal)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                vehiculos.Add(new
                {
                    Matricula = reader.GetNullableString(0),
                    Color = reader.GetNullableString(1),
                    Marca = reader.GetNullableString(2)
                });
            }
        }

        var ordenes = new List<dynamic>();
        try
        {
            await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync("Get_OrdenesPorCliente",
                ("@idCliente", idFinal),
                ("@fechaDesde", DBNull.Value),
                ("@fechaHasta", DBNull.Value),
                ("@orEstado", DBNull.Value)))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    ordenes.Add(new
                    {
                        IdOrden = reader.GetInt32(0),
                        FechaCreacion = reader.GetDateTime(1),
                        Estado = reader.GetNullableString(2),
                        Tipo = reader.GetNullableString(3),
                        FechaEntrega = reader.GetNullableDateTime(4),
                        FechaFin = reader.GetNullableDateTime(5),
                        CostoTotal = reader.GetNullableDouble(6) ?? 0.0,
                        VehiculoMatric = reader.GetNullableString(7),
                        VehiculoMarca = reader.GetNullableString(8)
                    });
                }
            }
        }
        catch (Microsoft.Data.SqlClient.SqlException)
        {
            // El procedimiento Get_OrdenesPorCliente arroja RAISERROR si el cliente está inactivo
        }

        ViewBag.Cliente = cliente;
        ViewBag.Vehiculos = vehiculos;
        ViewBag.Ordenes = ordenes;

        return View();
    }

    [Authorize(Roles = RolesSistema.Gestion)]
    [HttpPost("Editar/{idCliente}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        int idCliente,
        string pNombre,
        string pApellido,
        string pTelefono,
        string pCedula,
        string clEmail)
    {
        await _bdTallerContext.Database.ExecuteStoredProcedureAsync("Upd_Cliente",
            ("@idCliente", idCliente),
            ("@pNombre", pNombre),
            ("@pApellido", pApellido),
            ("@pTelefono", pTelefono?.Replace("-", "") ?? string.Empty),
            ("@pCedula", pCedula),
            ("@clEmail", clEmail));

        return RedirectToAction(nameof(Lista));
    }

    [Authorize(Roles = RolesSistema.AdminSupervisor)]
    [HttpPost("CambiarEstado")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int idCliente)
    {
        await _bdTallerContext.Database.ExecuteStoredProcedureAsync("Mod_EstadoCl", ("@idCliente", idCliente));
        return RedirectToAction(nameof(Lista));
    }
}
