using System.Data;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using syswb_Taller.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers;

public class PresupuestoController : Controller
{
    private readonly BdTallerContext _bd;

    public PresupuestoController(BdTallerContext bd) => _bd = bd;

    #region CRUD_Presupuesto

    [HttpGet]
    [Authorize(Roles = RolesSistema.Todos)]
    public async Task<IActionResult> Lista(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? clienteID = null, bool? estado = null)
    {
        var presupuestos = new List<dynamic>();

        await using (var cmd = await _bd.Database.CreateStoredProcedureAsync("Get_Presupuestos",
            ("@FechaDesde", fechaDesde),
            ("@FechaHasta", fechaHasta),
            ("@ClienteID", clienteID),
            ("@Estado", estado)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                presupuestos.Add(new
                {
                    Id = reader.GetInt32(0),
                    Fecha = reader.GetDateTime(1),
                    Cliente = reader.GetString(2),
                    Estado = reader.GetBoolean(3),
                    Costo = reader.GetDouble(4)
                });
            }
        }

        ViewBag.FiltroFechaDesde = fechaDesde;
        ViewBag.FiltroFechaHasta = fechaHasta;
        ViewBag.FiltroCliente = clienteID;
        ViewBag.FiltroEstado = estado;
        return View(presupuestos);
    }

    [HttpGet]
    [Authorize(Roles = RolesSistema.Gestion)]
    public IActionResult CrearClienteVehiculo() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> CrearClienteVehiculo(
        string pNombre,
        string pApellido,
        string pTelefono,
        string pCedula,
        string clEmail,
        string vhMatricula,
        string vhMarca,
        string vhModelo,
        int vhAño,
        string vhColor,
        string vhEstado)
    {
        await _bd.Database.OpenConnectionAsync();
        var conn = _bd.Database.GetDbConnection();
        await using var tran = await conn.BeginTransactionAsync();

        int idCliente;
        await using (var cmd = conn.CreateStoredProcedure("Ins_Cliente", tran,
            ("@pNombre", pNombre),
            ("@pApellido", pApellido),
            ("@pTelefono", pTelefono),
            ("@pCedula", pCedula),
            ("@clEmail", clEmail)))
        {
            var outIdCliente = cmd.AddOutputParameter("@OutIdCliente", SqlDbType.Int);
            await cmd.ExecuteNonQueryAsync();
            idCliente = (int)outIdCliente.Value;
        }

        int idVehiculo;
        await using (var cmd = conn.CreateStoredProcedure("Ins_Vehiculo", tran,
            ("@vhMatricula", vhMatricula),
            ("@vhMarca", vhMarca),
            ("@vhModelo", vhModelo),
            ("@vhAño", vhAño),
            ("@vhColor", vhColor),
            ("@vhEstado", vhEstado),
            ("@idCliente", idCliente)))
        {
            var outIdVehiculo = cmd.AddOutputParameter("@idVehiculo", SqlDbType.Int);
            await cmd.ExecuteNonQueryAsync();
            idVehiculo = (int)outIdVehiculo.Value;
        }

        await tran.CommitAsync();

        return RedirectToAction(nameof(CrearPresupuesto), new { clienteID = idCliente, idVehiculo });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> ActualizarPresupuesto(int idPresupuesto, string preFalla)
    {
        var presupuesto = await _bd.Presupuestos.FindAsync(idPresupuesto);
        if (presupuesto == null)
            return NotFound();

        presupuesto.PreFalla = preFalla;
        await _bd.SaveChangesAsync();

        return RedirectToAction(nameof(Lista));
    }

    [HttpGet]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> EditarPresupuesto(int id)
    {
        var presupuesto = await _bd.Presupuestos.FindAsync(id);
        if (presupuesto == null)
            return NotFound();

        await CargarDatosFormularioAsync(presupuesto.IdCliente, presupuesto.IdVehiculo);

        // En la edición solo se ofrecen packs con precio calculado
        ViewBag.Packs = await _bd.PackReparaciones
            .Where(p => p.PckActivo && p.PckPrecio != null)
            .ToListAsync();

        var detalles = new List<dynamic>();

        var packs = await _bd.PresupuestoDetallePacks
            .Where(d => d.IdPresupuesto == id)
            .Include(d => d.IdPackNavigation)
            .ToListAsync();
        detalles.AddRange(packs.Select(d => new
        {
            Tipo = "Pack",
            Nombre = d.IdPackNavigation.PckNombre,
            Cantidad = d.PpCantidad,
            PrecioPack = d.IdPackNavigation.PckPrecio,
            d.IdDetalle
        }));

        var productos = await _bd.PresupuestoDetalleProductos
            .Where(d => d.IdPresupuesto == id)
            .Include(d => d.IdProductoNavigation)
            .ToListAsync();
        detalles.AddRange(productos.Select(d => new
        {
            Tipo = "Producto",
            Nombre = d.IdProductoNavigation.PrNombre,
            Cantidad = d.PpCantidad,
            PrecioUnit = d.IdProductoNavigation.PrPrecioVenta,
            d.IdDetalle
        }));

        var servicios = await _bd.PresupuestoDetalleServicios
            .Where(d => d.IdPresupuesto == id)
            .Include(d => d.IdServicioNavigation)
            .ToListAsync();
        detalles.AddRange(servicios.Select(d => new
        {
            Tipo = "Servicio",
            Nombre = d.IdServicioNavigation.SerNombre,
            Cantidad = d.PsCantidad,
            PrecioUnit = d.IdServicioNavigation.SerPrecioUnit,
            d.IdDetalle
        }));

        ViewBag.Detalles = detalles;

        return View("Crear", new
        {
            presupuesto.IdPresupuesto,
            Fecha = presupuesto.PreFechaCreacion,
            CostoTotal = presupuesto.PreCostoTotal,
            presupuesto.PreFalla,
            Estado = presupuesto.PreEstado,
            Detalles = detalles
        });
    }

    [HttpGet]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> SeleccionarClientePresupuesto(string? nombre = null, bool? activo = null)
    {
        var clientes = new List<dynamic>();

        await using (var cmd = await _bd.Database.CreateStoredProcedureAsync(
            "Get_Clientes", ("@Nombre", nombre), ("@Activo", activo)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                dynamic cliente = new ExpandoObject();
                cliente.Id = reader.GetInt32(0);
                cliente.Nombre = reader.GetString(1);
                cliente.Apellido = reader.GetString(2);
                cliente.Telefono = reader.GetString(3);
                cliente.Vehiculos = new List<dynamic>();
                clientes.Add(cliente);
            }
        }

        foreach (var cliente in clientes)
        {
            int idCliente = (int)cliente.Id;
            cliente.Vehiculos = await _bd.Vehiculos
                .Where(v => v.IdCliente == idCliente && v.VhActivo == true)
                .Select(v => new { v.IdVehiculo, v.VhMatricula, v.VhMarca })
                .ToListAsync();
        }

        ViewBag.FiltroNombre = nombre;
        ViewBag.FiltroActivo = activo;
        return View(clientes);
    }

    [HttpGet]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> CrearPresupuesto(int clienteID, int idVehiculo)
    {
        await CargarDatosFormularioAsync(clienteID, idVehiculo);

        ViewBag.Packs = await _bd.PackReparaciones.Where(p => p.PckActivo).ToListAsync();
        ViewBag.Detalles = new List<dynamic>();

        return View("Crear");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> CrearPresupuesto(int clienteID, int idVehiculo, string preFalla)
    {
        int idPresupuesto = 0;

        await using (var cmd = await _bd.Database.CreateStoredProcedureAsync("Ins_Presupuesto",
            ("@preFalla", preFalla),
            ("@idVehiculo", idVehiculo),
            ("@idCliente", clienteID)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            if (await reader.ReadAsync())
                idPresupuesto = Convert.ToInt32(reader["IdPresupuesto"]);
        }

        if (idPresupuesto == 0)
        {
            TempData["Error"] = "No se pudo crear el presupuesto.";
            return RedirectToAction(nameof(Lista));
        }

        return RedirigirAEditar(idPresupuesto);
    }

    #endregion

    #region Detalles

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> AgregarPack(int idPresupuesto, int idPack, int ppCantidad)
    {
        await _bd.Database.ExecuteStoredProcedureAsync("Ins_PresupuestoDetPack",
            ("@idPresupuesto", idPresupuesto),
            ("@idPack", idPack),
            ("@ppCantidad", ppCantidad));

        return RedirigirAEditar(idPresupuesto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> AgregarProducto(int idPresupuesto, int idProducto, int ppCantidad)
    {
        await _bd.Database.ExecuteStoredProcedureAsync("Ins_PresupuestoDetProducto",
            ("@idPresupuesto", idPresupuesto),
            ("@idProducto", idProducto),
            ("@ppCantidad", ppCantidad));

        return RedirigirAEditar(idPresupuesto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> AgregarServicio(int idPresupuesto, int idServicio, int psCantidad)
    {
        await _bd.Database.ExecuteStoredProcedureAsync("Ins_PresupuestoDetServicio",
            ("@idPresupuesto", idPresupuesto),
            ("@idServicio", idServicio),
            ("@psCantidad", psCantidad));

        return RedirigirAEditar(idPresupuesto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> ActualizarPack(int idDetalle, int? ppCantidad)
    {
        await _bd.Database.ExecuteStoredProcedureAsync("Upd_PresupuestoDetPack",
            ("@idDetalle", idDetalle),
            ("@ppCantidad", ppCantidad));

        int idPresupuesto = await _bd.PresupuestoDetallePacks
            .Where(d => d.IdDetalle == idDetalle)
            .Select(d => d.IdPresupuesto)
            .FirstAsync();

        return RedirigirAEditar(idPresupuesto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> ActualizarProducto(int idDetalle, int? ppCantidad)
    {
        await _bd.Database.ExecuteStoredProcedureAsync("Upd_PresupuestoDetProducto",
            ("@idDetalle", idDetalle),
            ("@ppCantidad", ppCantidad));

        int idPresupuesto = await _bd.PresupuestoDetalleProductos
            .Where(d => d.IdDetalle == idDetalle)
            .Select(d => d.IdPresupuesto)
            .FirstAsync();

        return RedirigirAEditar(idPresupuesto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> ActualizarServicio(int idDetalle, int? psCantidad)
    {
        await _bd.Database.ExecuteStoredProcedureAsync("Upd_PresupuestoDetServicio",
            ("@idDetalle", idDetalle),
            ("@psCantidad", psCantidad));

        int idPresupuesto = await _bd.PresupuestoDetalleServicios
            .Where(d => d.IdDetalle == idDetalle)
            .Select(d => d.IdPresupuesto)
            .FirstAsync();

        return RedirigirAEditar(idPresupuesto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> EliminarPack(int idDetalle)
    {
        int idPresupuesto = await _bd.PresupuestoDetallePacks
            .Where(d => d.IdDetalle == idDetalle)
            .Select(d => d.IdPresupuesto)
            .FirstAsync();

        await _bd.Database.ExecuteStoredProcedureAsync("Del_PresupuestoDetPack", ("@idDetalle", idDetalle));

        return RedirigirAEditar(idPresupuesto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> EliminarProducto(int idDetalle)
    {
        int idPresupuesto = await _bd.PresupuestoDetalleProductos
            .Where(d => d.IdDetalle == idDetalle)
            .Select(d => d.IdPresupuesto)
            .FirstAsync();

        await _bd.Database.ExecuteStoredProcedureAsync("Del_PresupuestoDetProducto", ("@idDetalle", idDetalle));

        return RedirigirAEditar(idPresupuesto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> EliminarServicio(int idDetalle)
    {
        int idPresupuesto = await _bd.PresupuestoDetalleServicios
            .Where(d => d.IdDetalle == idDetalle)
            .Select(d => d.IdPresupuesto)
            .FirstAsync();

        await _bd.Database.ExecuteStoredProcedureAsync("Del_PresupuestoDetServicio", ("@idDetalle", idDetalle));

        return RedirigirAEditar(idPresupuesto);
    }

    #endregion

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> GenerarOrdenDesdePresupuesto(int idPresupuesto)
    {
        if (await _bd.Ordenes.AnyAsync(o => o.IdPresupuesto == idPresupuesto))
        {
            TempData["Error"] = "Este presupuesto ya tiene una orden generada.";
            return RedirigirAEditar(idPresupuesto);
        }

        var presupuesto = await _bd.Presupuestos.FindAsync(idPresupuesto);
        if (presupuesto == null)
        {
            TempData["Error"] = "Presupuesto no encontrado.";
            return RedirectToAction(nameof(Lista));
        }

        if (presupuesto.IdVehiculo == 0)
        {
            TempData["Error"] = "No puede generar orden sin un vehículo asignado.";
            return RedirigirAEditar(idPresupuesto);
        }

        int idOrden = 0;
        try
        {
            await using var cmd = await _bd.Database.CreateStoredProcedureAsync(
                "Ins_OrdenDesdePresupuesto", ("@idPresupuesto", idPresupuesto));
            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                idOrden = reader.GetInt32(0);
        }
        catch (SqlException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirigirAEditar(idPresupuesto);
        }

        if (idOrden == 0)
        {
            TempData["Error"] = "No se pudo generar la orden desde el presupuesto.";
            return RedirigirAEditar(idPresupuesto);
        }

        presupuesto.PreEstado = true;
        await _bd.SaveChangesAsync();

        TempData["Success"] = "Orden generada correctamente.";
        return RedirectToAction(nameof(OrdenController.Editar), "Orden", new { id = idOrden });
    }

    #region Auxiliares

    private RedirectToActionResult RedirigirAEditar(int idPresupuesto) =>
        RedirectToAction(nameof(EditarPresupuesto), new { id = idPresupuesto });

    /// <summary>
    /// Datos comunes del formulario "Crear": cliente, vehículo, productos y servicios activos.
    /// </summary>
    private async Task CargarDatosFormularioAsync(int? idCliente, int? idVehiculo)
    {
        var cliente = await _bd.Clientes
            .Include(c => c.IdPersonaNavigation)
            .FirstOrDefaultAsync(c => c.IdCliente == idCliente);
        var vehiculo = await _bd.Vehiculos.FindAsync(idVehiculo);

        ViewBag.ClienteID = cliente?.IdCliente ?? idCliente;
        ViewBag.IdVehiculo = vehiculo?.IdVehiculo ?? idVehiculo;
        ViewBag.ClienteNombre = $"{cliente?.IdPersonaNavigation.PNombre} {cliente?.IdPersonaNavigation.PApellido}";
        ViewBag.Matricula = vehiculo?.VhMatricula;

        ViewBag.Productos = await _bd.ProductoRepuestos.Where(p => p.PrActivo).ToListAsync();
        ViewBag.Servicios = await _bd.Servicios.Where(s => s.SerActivo).ToListAsync();
    }

    #endregion
}
