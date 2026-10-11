using System.Data;
using System.Dynamic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using syswb_Taller.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers;

public class OrdenController : Controller
{
    private const string RolMecanicoBd = "Mecánico";

    private static readonly string[] Tipos = { "Mantenimiento", "Reparación", "Inspección" };

    private readonly BdTallerContext _bd;

    public OrdenController(BdTallerContext bd) => _bd = bd;

    #region CRUD_Orden

    [HttpGet]
    [Authorize(Roles = RolesSistema.Todos)]
    public async Task<IActionResult> Lista(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? clienteID = null, string? estado = null)
    {
        var ordenes = new List<dynamic>();

        await using (var cmd = await _bd.Database.CreateStoredProcedureAsync("Get_Ordenes",
            ("@FechaDesde", fechaDesde),
            ("@FechaHasta", fechaHasta),
            ("@ClienteID", clienteID),
            ("@Estado", estado)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                ordenes.Add(new
                {
                    Id = reader.GetInt32(0),
                    Creacion = reader.GetDateTime(1),
                    Cliente = reader.GetString(2),
                    Vehiculo = reader.GetString(3),
                    Entrega = reader.GetNullableDateTime(4),
                    Costo = reader.GetDouble(5),
                    Pagada = reader.GetInt32(6) == 1,
                    Estado = reader.GetString(7)
                });
            }
        }

        ViewBag.FiltroFechaDesde = fechaDesde;
        ViewBag.FiltroFechaHasta = fechaHasta;
        ViewBag.FiltroCliente = clienteID;
        ViewBag.FiltroEstado = estado;
        return View(ordenes);
    }

    [HttpGet]
    [Authorize(Roles = RolesSistema.Gestion)]
    public IActionResult CrearClienteVehiculo() => View();

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> CrearClienteVehiculo(
        string pNombre, string pApellido, string pTelefono, string pCedula, string clEmail,
        string vhMatricula, string vhMarca, string vhModelo, int vhAño, string vhColor, string vhEstado)
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

        return RedirectToAction(nameof(Crear), new { clienteID = idCliente, vehiculoID = idVehiculo });
    }

    [HttpGet]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> SeleccionarClienteOrden(string? nombre = null, bool? activo = null)
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
    public async Task<IActionResult> Crear(int clienteID, int vehiculoID, int? presupuestoID = null)
    {
        ViewBag.ClienteID = clienteID;
        ViewBag.VehiculoID = vehiculoID;
        ViewBag.PresupuestoID = presupuestoID;

        ViewBag.Estados = new[] { "Pendiente", "En Proceso", "Entregado" };
        ViewBag.Tipos = Tipos;
        ViewBag.Mecanicos = await ObtenerEmpleadosActivosAsync(soloMecanicos: true);

        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Crear(
        int clienteID,
        int vehiculoID,
        string orEstado,
        string orTipo,
        DateTime orFechaEntrega,
        DateTime? orFechaFin,
        string orFalla,
        int? presupuestoID,
        int? idMecanicoTemp)
    {
        await _bd.Database.ExecuteStoredProcedureAsync("Ins_Orden",
            ("@orEstado", orEstado),
            ("@orTipo", orTipo),
            ("@orFechaEntrega", orFechaEntrega),
            ("@orFechaFin", orFechaFin),
            ("@orFalla", orFalla),
            ("@idCliente", clienteID),
            ("@idVehiculo", vehiculoID),
            ("@idPresupuesto", presupuestoID));

        if (idMecanicoTemp.HasValue)
            TempData["MecanicoTemp"] = idMecanicoTemp.Value;

        return RedirectToAction(nameof(Lista));
    }

    [HttpGet]
    [Authorize(Roles = RolesSistema.Todos)]
    public async Task<IActionResult> Editar(int id)
    {
        var orden = await _bd.Ordenes.FindAsync(id);
        if (orden == null)
            return NotFound();

        var cliente = await _bd.Clientes
            .Include(c => c.IdPersonaNavigation)
            .FirstAsync(c => c.IdCliente == orden.IdCliente);
        var vehiculo = await _bd.Vehiculos.FindAsync(orden.IdVehiculo);

        ViewBag.ClienteID = cliente.IdCliente;
        ViewBag.VehiculoID = vehiculo!.IdVehiculo;
        ViewBag.ClienteNombre = $"{cliente.IdPersonaNavigation.PNombre} {cliente.IdPersonaNavigation.PApellido}";
        ViewBag.Matricula = vehiculo.VhMatricula;

        ViewBag.Estados = new[] { "Pendiente", "En Proceso", "Finalizado" };
        ViewBag.Tipos = Tipos;
        ViewBag.Packs = await _bd.PackReparaciones.Where(p => p.PckActivo).ToListAsync();
        ViewBag.Productos = await _bd.ProductoRepuestos.Where(p => p.PrActivo).ToListAsync();
        ViewBag.Servicios = await _bd.Servicios.Where(s => s.SerActivo).ToListAsync();

        var detalles = new List<dynamic>();

        var packs = await _bd.OrdenDetallePacks
            .Include(d => d.IdPackNavigation)
            .Where(d => d.IdOrden == id)
            .ToListAsync();
        detalles.AddRange(packs.Select(d => new
        {
            Tipo = "Pack",
            Nombre = d.IdPackNavigation.PckNombre,
            Cantidad = d.OpCantidad,
            Precio = d.OpPrecioPack,
            d.IdDetalle
        }));

        var productos = await _bd.OrdenDetalleProductos
            .Include(d => d.IdProductoNavigation)
            .Where(d => d.IdOrden == id)
            .ToListAsync();
        detalles.AddRange(productos.Select(d => new
        {
            Tipo = "Producto",
            Nombre = d.IdProductoNavigation.PrNombre,
            Cantidad = d.OpCantidad,
            Precio = d.OpPrecioUnit,
            d.IdDetalle
        }));

        var servicios = await _bd.OrdenDetalleServicios
            .Include(d => d.IdServicioNavigation)
            .Where(d => d.IdOrden == id)
            .ToListAsync();
        detalles.AddRange(servicios.Select(d => new
        {
            Tipo = "Servicio",
            Nombre = d.IdServicioNavigation.SerNombre,
            Cantidad = d.OpCantidad,
            Precio = d.OpPrecioUnit,
            d.IdDetalle
        }));

        ViewBag.Detalles = detalles;
        ViewBag.Mecanicos = await ObtenerEmpleadosActivosAsync(soloMecanicos: true);
        ViewBag.YaPago = await _bd.DetallePagoOrdens.AnyAsync(dp => dp.IdOrden == id);
        ViewBag.CostoTotal = orden.OrCostoTotal;

        return View(nameof(Crear), orden);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Editar(
        int idOrden,
        string orEstado,
        string orTipo,
        DateTime orFechaEntrega,
        DateTime? orFechaFin,
        string orFalla,
        int? idPresupuesto,
        int? idMecanicoTemp)
    {
        await _bd.Database.ExecuteStoredProcedureAsync("Upd_Orden",
            ("@idOrden", idOrden),
            ("@orEstado", orEstado),
            ("@orTipo", orTipo),
            ("@orFechaEntrega", orFechaEntrega),
            ("@orFechaFin", orFechaFin),
            ("@orFalla", orFalla),
            ("@idPresupuesto", idPresupuesto));

        if (idMecanicoTemp.HasValue)
            TempData["MecanicoTemp"] = idMecanicoTemp.Value;

        return RedirectToAction(nameof(Lista));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public Task<IActionResult> AgregarPack(int idOrden, int idPack, int opCantidad) =>
        EjecutarDetalleAsync(idOrden, "Ins_OrdenDetPack", null,
            ("@idOrden", idOrden), ("@idPack", idPack), ("@opCantidad", opCantidad));

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public Task<IActionResult> AgregarProducto(int idOrden, int idProducto, int opCantidad) =>
        EjecutarDetalleAsync(idOrden, "Ins_OrdenDetProducto", null,
            ("@idOrden", idOrden), ("@idProducto", idProducto), ("@opCantidad", opCantidad));

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public Task<IActionResult> AgregarServicio(int idOrden, int idServicio, int opCantidad) =>
        EjecutarDetalleAsync(idOrden, "Ins_OrdenDetServicio", null,
            ("@idOrden", idOrden), ("@idServicio", idServicio), ("@opCantidad", opCantidad));

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> ActualizarPack(int idDetalle, int? opCantidad)
    {
        int idOrden = await _bd.OrdenDetallePacks.Where(d => d.IdDetalle == idDetalle).Select(d => d.IdOrden).FirstAsync();
        return await EjecutarDetalleAsync(idOrden, "Upd_OrdenDetPack",
            "No hay stock suficiente para uno o más componentes del pack.",
            ("@idDetalle", idDetalle), ("@opCantidad", opCantidad));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> ActualizarProducto(int idDetalle, int? opCantidad)
    {
        int idOrden = await _bd.OrdenDetalleProductos.Where(d => d.IdDetalle == idDetalle).Select(d => d.IdOrden).FirstAsync();
        return await EjecutarDetalleAsync(idOrden, "Upd_OrdenDetProducto",
            "No hay suficiente stock disponible para este producto.",
            ("@idDetalle", idDetalle), ("@opCantidad", opCantidad));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> ActualizarServicio(int idDetalle, int? opCantidad)
    {
        int idOrden = await _bd.OrdenDetalleServicios.Where(d => d.IdDetalle == idDetalle).Select(d => d.IdOrden).FirstAsync();
        return await EjecutarDetalleAsync(idOrden, "Upd_OrdenDetServicio", null,
            ("@idDetalle", idDetalle), ("@opCantidad", opCantidad));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.AdminSupervisor)]
    public async Task<IActionResult> EliminarPack(int idDetalle)
    {
        int idOrden = await _bd.OrdenDetallePacks.Where(d => d.IdDetalle == idDetalle).Select(d => d.IdOrden).FirstAsync();
        return await EjecutarDetalleAsync(idOrden, "Del_OrdenDetPack", null, ("@idDetalle", idDetalle));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> EliminarProducto(int idDetalle)
    {
        int idOrden = await _bd.OrdenDetalleProductos.Where(d => d.IdDetalle == idDetalle).Select(d => d.IdOrden).FirstAsync();
        return await EjecutarDetalleAsync(idOrden, "Del_OrdenDetProducto", null, ("@idDetalle", idDetalle));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> EliminarServicio(int idDetalle)
    {
        int idOrden = await _bd.OrdenDetalleServicios.Where(d => d.IdDetalle == idDetalle).Select(d => d.IdOrden).FirstAsync();
        return await EjecutarDetalleAsync(idOrden, "Del_OrdenDetServicio", null, ("@idDetalle", idDetalle));
    }

    #endregion

    #region Pago_Entrega

    [HttpGet]
    [Authorize(Roles = RolesSistema.Todos)]
    public async Task<IActionResult> AgregarEntrega(int idOrden)
    {
        var orden = await _bd.Ordenes.FindAsync(idOrden);
        if (orden == null)
            return NotFound();

        var entrega = await _bd.OrdenDetalleEntregas.FirstOrDefaultAsync(e => e.IdOrden == idOrden);
        bool esCreacion = entrega == null;
        entrega ??= new OrdenDetalleEntrega { IdOrden = idOrden };

        ViewBag.EmpleadosActivos = await ObtenerEmpleadosActivosAsync(soloMecanicos: false);
        ViewBag.Mecanicos = await ObtenerEmpleadosActivosAsync(soloMecanicos: true);
        ViewBag.MecanicoAsignado = esCreacion ? TempData["MecanicoTemp"] : entrega.MecanicoResponsable;
        ViewBag.EsCreacion = esCreacion;

        return View("EntregaOrden", entrega);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Todos)]
    public async Task<IActionResult> AgregarEntrega(
        int idOrden,
        string recibido_por,
        string telefono_recibe,
        bool es_propietario,
        int entregado_por,
        string orden_entrega,
        int mecanico_responsable,
        string informe_tecnico)
    {
        await _bd.Database.ExecuteStoredProcedureAsync("Ins_OrdenDetEntrega",
            ("@idOrden", idOrden),
            ("@recibido_por", recibido_por),
            ("@telefono_recibe", telefono_recibe),
            ("@es_propietario", es_propietario),
            ("@entregado_por", entregado_por),
            ("@orden_entrega", orden_entrega),
            ("@mecanico_responsable", mecanico_responsable),
            ("@informe_tecnico", informe_tecnico));

        return RedirigirAEditar(idOrden);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Todos)]
    public async Task<IActionResult> ActualizarEntrega(
        int idEntrega,
        string? recibido_por = null,
        string? telefono_recibe = null,
        bool? es_propietario = null,
        int? entregado_por = null,
        DateTime? fecha_entrega = null,
        string? orden_entrega = null,
        int? mecanico_responsable = null,
        string? informe_tecnico = null)
    {
        await _bd.Database.ExecuteStoredProcedureAsync("Upd_OrdenDetEntrega",
            ("@idEntrega", idEntrega),
            ("@recibido_por", recibido_por),
            ("@telefono_recibe", telefono_recibe),
            ("@es_propietario", es_propietario),
            ("@entregado_por", entregado_por),
            ("@fecha_entrega", fecha_entrega),
            ("@orden_entrega", orden_entrega),
            ("@mecanico_responsable", mecanico_responsable),
            ("@informe_tecnico", informe_tecnico));

        int idOrden = await _bd.OrdenDetalleEntregas.Where(d => d.IdEntrega == idEntrega).Select(d => d.IdOrden).FirstAsync();
        return RedirigirAEditar(idOrden);
    }

    [HttpGet]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> AgregarPagoOrden(int idOrden)
    {
        var orden = await _bd.Ordenes
            .Include(o => o.DetallePagoOrdens)
            .Include(o => o.IdVehiculoNavigation)
            .FirstOrDefaultAsync(o => o.IdOrden == idOrden);

        if (orden == null)
            return NotFound();

        if (orden.OrCostoTotal == 0)
        {
            TempData["Error"] = "No puede pagar una orden de costo 0.";
            return RedirigirAEditar(idOrden);
        }

        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var idEmpleado))
        {
            TempData["Error"] = "No se pudo obtener la identidad del empleado.";
            return RedirigirAEditar(idOrden);
        }

        var pagoExistente = orden.DetallePagoOrdens.FirstOrDefault();
        var model = pagoExistente ?? new DetallePagoOrden
        {
            IdOrden = idOrden,
            MontoPagado = orden.OrCostoTotal
        };

        var empleado = await _bd.Empleados
            .Include(e => e.IdPersonaNavigation)
            .FirstOrDefaultAsync(e => e.IdEmpleado == idEmpleado);

        ViewBag.YaPago = pagoExistente != null;
        ViewBag.ClienteID = orden.IdVehiculoNavigation?.IdCliente;
        ViewBag.EmpleadoNombre = empleado != null
            ? $"{empleado.IdPersonaNavigation.PNombre} {empleado.IdPersonaNavigation.PApellido}"
            : "Empleado no identificado";
        ViewBag.FechaHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> AgregarPagoOrden(
        int idOrden,
        string metodoPago,
        string? observaciones,
        int idCliente,
        string orden_entrega)
    {
        int idEmpleado = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var orden = await _bd.Ordenes.FindAsync(idOrden);
        if (orden == null)
        {
            TempData["Error"] = "Orden no encontrada.";
            return RedirigirAEditar(idOrden);
        }

        if (orden.OrCostoTotal == 0)
        {
            TempData["Error"] = "No puede realizar pagos en una orden cuyo costo es 0.";
            return RedirigirAEditar(idOrden);
        }

        if (await _bd.DetallePagoOrdens.AnyAsync(dp => dp.IdOrden == idOrden))
        {
            TempData["Error"] = "Esta orden ya tiene un pago registrado.";
            return RedirigirAEditar(idOrden);
        }

        await _bd.Database.ExecuteStoredProcedureAsync("Ins_DetallePagoOrden",
            ("@idOrden", idOrden),
            ("@metodoPago", metodoPago),
            ("@observaciones", observaciones),
            ("@idEmpleado", idEmpleado),
            ("@idCliente", idCliente));

        return RedirectToAction(nameof(DetallesPago), new { idOrden });
    }

    // GET: Orden/DetallesEntrega/5
    [HttpGet]
    [Authorize(Roles = "Administrador, Cajero, Supervisor, Mecánico")]
    public async Task<IActionResult> DetallesEntrega(int idOrden)
    {
        var entrega = await _bd.OrdenDetalleEntregas.FirstOrDefaultAsync(e => e.IdOrden == idOrden);

        // Si todavía no existe, se va al formulario de creación
        if (entrega == null)
            return RedirectToAction(nameof(AgregarEntrega), new { idOrden });

        return View(nameof(DetallesEntrega), entrega);
    }

    // GET: Orden/DetallesPago/5
    [HttpGet]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> DetallesPago(int idOrden)
    {
        var pago = await _bd.DetallePagoOrdens.FirstOrDefaultAsync(p => p.IdOrden == idOrden);
        if (pago == null)
            return RedirectToAction(nameof(AgregarPagoOrden), new { idOrden });

        return View(nameof(DetallesPago), pago);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> ActualizarPagoOrden(
        int idPagoOrden,
        string? metodoPago = null,
        string? observaciones = null)
    {
        await _bd.Database.ExecuteStoredProcedureAsync("Upd_DetallePagoOrden",
            ("@idPagoOrden", idPagoOrden),
            ("@metodoPago", metodoPago),
            ("@observaciones", observaciones));

        int idOrden = await _bd.DetallePagoOrdens.Where(d => d.IdPagoOrden == idPagoOrden).Select(d => d.IdOrden).FirstAsync();
        return RedirigirAEditar(idOrden);
    }

    #endregion

    [HttpGet]
    [Authorize(Roles = RolesSistema.Mecanico)]
    public IActionResult AbrirDetalleEntrega() => RedirectToAction(nameof(AgregarEntrega), "Orden");

    #region Auxiliares

    private RedirectToActionResult RedirigirAEditar(int idOrden) =>
        RedirectToAction(nameof(Editar), new { id = idOrden });

    /// <summary>
    /// Ejecuta un procedimiento sobre los detalles de la orden y vuelve a la pantalla de edición.
    /// Los errores de SQL se muestran en TempData["Error"]; si se indica <paramref name="mensajeSinStock"/>,
    /// reemplaza el mensaje original cuando el error es por stock insuficiente.
    /// </summary>
    private async Task<IActionResult> EjecutarDetalleAsync(
        int idOrden,
        string procedimiento,
        string? mensajeSinStock,
        params (string Name, object? Value)[] parametros)
    {
        try
        {
            await _bd.Database.ExecuteStoredProcedureAsync(procedimiento, parametros);
        }
        catch (SqlException ex)
        {
            TempData["Error"] = mensajeSinStock != null && ex.Message.Contains("Stock insuficiente")
                ? mensajeSinStock
                : ex.Message;
        }

        return RedirigirAEditar(idOrden);
    }

    private async Task<List<EmpleadoOpcion>> ObtenerEmpleadosActivosAsync(bool soloMecanicos)
    {
        var consulta = _bd.Empleados.Where(e => e.EmpActivo);

        if (soloMecanicos)
            consulta = consulta.Where(e => e.IdRolNavigation.RTipo == RolMecanicoBd);

        return await consulta
            .Select(e => new EmpleadoOpcion(
                e.IdEmpleado,
                e.IdPersonaNavigation.PNombre + " " + e.IdPersonaNavigation.PApellido))
            .ToListAsync();
    }

    #endregion
}
