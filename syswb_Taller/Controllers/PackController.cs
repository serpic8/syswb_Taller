using System.Data;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using syswb_Taller.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers;

public class PackController : Controller
{
    private const string ErrorYaAsignado = "ya está asignado al pack";

    private readonly BdTallerContext _bdTallerContext;

    public PackController(BdTallerContext bdTallerContext)
    {
        _bdTallerContext = bdTallerContext;
    }

    #region CRUD Pack

    [HttpGet]
    [Authorize(Roles = RolesSistema.Todos)]
    public async Task<IActionResult> Lista(string? nombre = null, bool? activo = null)
    {
        var packs = new List<dynamic>();

        await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync(
            "Get_Packs", ("@Nombre", nombre), ("@Activo", activo)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                packs.Add(new
                {
                    IdPack = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Precio = reader.GetNullableDouble(2) ?? 0.0,
                    Activo = reader.GetBoolean(3)
                });
            }
        }

        ViewBag.FiltroNombre = nombre;
        ViewBag.FiltroActivo = activo;
        ViewBag.Packs = packs;
        return View();
    }

    [HttpGet]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Crear()
    {
        await CargarProductosYServiciosAsync();

        ViewBag.Detalles = new List<dynamic>();
        ViewBag.CostoTotal = 0.0;
        ViewBag.IdPack = 0;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Crear(string pckNombre, string? pckDescripcion)
    {
        await using var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync("Ins_PackReparacion",
            ("@pckNombre", pckNombre),
            ("@pckDescripcion", pckDescripcion));
        var outNewPackId = cmd.AddOutputParameter("@NewPackId", SqlDbType.Int);

        await cmd.ExecuteNonQueryAsync();

        if (outNewPackId.Value == DBNull.Value)
        {
            TempData["Error"] = "No fue posible crear el pack.";
            return RedirectToAction(nameof(Lista));
        }

        return RedirectToAction(nameof(Editar), new { id = (int)outNewPackId.Value });
    }

    [HttpGet]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Editar(int id)
    {
        var pack = await _bdTallerContext.PackReparaciones.FindAsync(id);
        if (pack == null)
            return NotFound();

        ViewBag.Pack = new
        {
            pack.IdPack,
            pack.PckNombre,
            pack.PckDescripcion
        };

        await CargarProductosYServiciosAsync();
        await CargarDetallesAsync(id);

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Editar(int idPack, string pckNombre, string? pckDescripcion)
    {
        await _bdTallerContext.Database.ExecuteStoredProcedureAsync("Upd_PackReparacion",
            ("@idPack", idPack),
            ("@pckNombre", pckNombre),
            ("@pckDescripcion", pckDescripcion));

        return RedirectToAction(nameof(Lista));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.AdminSupervisor)]
    public async Task<IActionResult> CambiarEstado(int idPack)
    {
        await _bdTallerContext.Database.ExecuteStoredProcedureAsync("Mod_EstadoPack", ("@idPack", idPack));
        return RedirectToAction(nameof(Lista));
    }

    #endregion

    #region CRUD Detalles

    [HttpGet]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Detalles(int idPack)
    {
        var pack = await _bdTallerContext.PackReparaciones.FindAsync(idPack);
        if (pack == null)
            return NotFound();

        ViewBag.Pack = pack;

        await CargarProductosYServiciosAsync();
        await CargarDetallesAsync(idPack);

        return View(nameof(Editar));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> AgregarProducto(int idPack, int idProducto, int pdCantidad = 1)
    {
        try
        {
            await _bdTallerContext.Database.ExecuteStoredProcedureAsync("Ins_DetallePackProducto",
                ("@idPack", idPack),
                ("@idProducto", idProducto),
                ("@pdCantidad", pdCantidad));
        }
        catch (SqlException ex) when (ex.Message.Contains(ErrorYaAsignado))
        {
            TempData["Error"] = "El producto ya está asignado a este pack.";
        }

        return RedirigirADetalles(idPack);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> AgregarServicio(int idPack, int idServicio, int psCantidad = 1)
    {
        try
        {
            await _bdTallerContext.Database.ExecuteStoredProcedureAsync("Ins_DetallePackServicio",
                ("@idPack", idPack),
                ("@idServicio", idServicio),
                ("@psCantidad", psCantidad));
        }
        catch (SqlException ex) when (ex.Message.Contains(ErrorYaAsignado))
        {
            TempData["Error"] = "El servicio ya está asignado a este pack.";
        }

        return RedirigirADetalles(idPack);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> ActualizarProducto(int idDetalle, int? pdCantidad)
    {
        var idPack = await ObtenerIdPackDeProductoAsync(idDetalle);

        await _bdTallerContext.Database.ExecuteStoredProcedureAsync("Upd_DetallePackProducto",
            ("@idDetalle", idDetalle),
            ("@pdCantidad", pdCantidad));

        return RedirigirADetalles(idPack);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> ActualizarServicio(int idDetalle, int? psCantidad)
    {
        var idPack = await ObtenerIdPackDeServicioAsync(idDetalle);

        await _bdTallerContext.Database.ExecuteStoredProcedureAsync("Upd_DetallePackServicio",
            ("@idDetalle", idDetalle),
            ("@psCantidad", psCantidad));

        return RedirigirADetalles(idPack);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> EliminarProducto(int idDetalle)
    {
        var idPack = await ObtenerIdPackDeProductoAsync(idDetalle);

        await _bdTallerContext.Database.ExecuteStoredProcedureAsync("Del_DetallePackProducto", ("@idDetalle", idDetalle));

        return RedirigirADetalles(idPack);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> EliminarServicio(int idDetalle)
    {
        var idPack = await ObtenerIdPackDeServicioAsync(idDetalle);

        await _bdTallerContext.Database.ExecuteStoredProcedureAsync("Del_DetallePackServicio", ("@idDetalle", idDetalle));

        return RedirigirADetalles(idPack);
    }

    #endregion

    #region Auxiliares

    private RedirectToActionResult RedirigirADetalles(int? idPack) =>
        RedirectToAction(nameof(Detalles), new { idPack });

    private Task<int?> ObtenerIdPackDeProductoAsync(int idDetalle) =>
        _bdTallerContext.DetallePackProductos
            .Where(d => d.IdDetalle == idDetalle)
            .Select(d => d.IdPack)
            .FirstAsync();

    private Task<int?> ObtenerIdPackDeServicioAsync(int idDetalle) =>
        _bdTallerContext.DetallePackServicios
            .Where(d => d.IdDetalle == idDetalle)
            .Select(d => d.IdPack)
            .FirstAsync();

    /// <summary>Productos y servicios activos para los desplegables.</summary>
    private async Task CargarProductosYServiciosAsync()
    {
        ViewBag.Productos = await _bdTallerContext.ProductoRepuestos
            .Where(p => p.PrActivo)
            .AsNoTracking()
            .ToListAsync();

        ViewBag.Servicios = await _bdTallerContext.Servicios
            .Where(s => s.SerActivo)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>Carga los detalles del pack (Get_DetallePack) y su costo total en ViewBag.</summary>
    private async Task CargarDetallesAsync(int idPack)
    {
        var detalles = new List<dynamic>();
        double costo = 0;

        await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync(
            "Get_DetallePack", ("@idPack", idPack)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                int cantidad = reader.GetInt32(3);
                double precio = reader.GetDouble(4);

                // ExpandoObject para que Razor acceda a las propiedades con dynamic
                dynamic detalle = new ExpandoObject();
                detalle.Tipo = reader.GetString(0);
                detalle.Id = reader.GetInt32(1);
                detalle.Nombre = reader.GetString(2);
                detalle.Cantidad = cantidad;
                detalle.Precio = precio;

                detalles.Add(detalle);
                costo += cantidad * precio;
            }
        }

        ViewBag.Detalles = detalles;
        ViewBag.CostoTotal = costo;
        ViewBag.IdPack = idPack;
    }

    #endregion
}
