using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Dynamic;
using System.Security.Claims;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers
{
    public class OrdenController : Controller
    {
        private readonly BdTallerContext _bd;
        public OrdenController(BdTallerContext bd) => _bd = bd;

        #region CRUD_Orden

        // 1) LISTAR ÓRDENES
        [HttpGet]
        [Authorize(Roles = "Administrador, Cajero, Supervisor, Mecanico")]
        public async Task<IActionResult> Lista(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? clienteID = null, string estado = null)
        {
            var list = new List<dynamic>();
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Get_Ordenes";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@FechaDesde", (object)fechaDesde ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@FechaHasta", (object)fechaHasta ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@ClienteID", (object)clienteID ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Estado", (object)estado ?? DBNull.Value));

            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new
                {
                    Id = rd.GetInt32(0),
                    Creacion = rd.GetDateTime(1),
                    Cliente = rd.GetString(2),
                    Vehiculo = rd.GetString(3),
                    Entrega = rd.IsDBNull(4) ? (DateTime?)null : rd.GetDateTime(4),
                    Costo = rd.GetDouble(5),
                    Pagada = rd.GetInt32(6) == 1,
                    Estado = rd.GetString(7)
                });
            }


            ViewBag.FiltroFechaDesde = fechaDesde;
            ViewBag.FiltroFechaHasta = fechaHasta;
            ViewBag.FiltroCliente = clienteID;
            ViewBag.FiltroEstado = estado;
            return View(list);
        }


        [HttpGet]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public IActionResult CrearClienteVehiculo()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> CrearClienteVehiculo(
            string pNombre, string pApellido, string pTelefono, string pCedula, string clEmail,
            string vhMatricula, string vhMarca, string vhModelo, int vhAño, string vhColor, string vhEstado
        )
        {
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using var tran = conn.BeginTransaction();

            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = "Ins_Cliente";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre));
                cmd.Parameters.Add(new SqlParameter("@pApellido", pApellido));
                cmd.Parameters.Add(new SqlParameter("@pTelefono", pTelefono));
                cmd.Parameters.Add(new SqlParameter("@pCedula", pCedula));
                cmd.Parameters.Add(new SqlParameter("@clEmail", clEmail));
                await cmd.ExecuteNonQueryAsync();
            }
            int idCliente = Convert.ToInt32(await new SqlCommand("SELECT SCOPE_IDENTITY()", (SqlConnection)conn, (SqlTransaction)tran).ExecuteScalarAsync());

            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = "Ins_Vehiculo";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@vhMatricula", vhMatricula));
                cmd.Parameters.Add(new SqlParameter("@vhMarca", vhMarca));
                cmd.Parameters.Add(new SqlParameter("@vhModelo", vhModelo));
                cmd.Parameters.Add(new SqlParameter("@vhAño", vhAño));
                cmd.Parameters.Add(new SqlParameter("@vhColor", vhColor));
                cmd.Parameters.Add(new SqlParameter("@vhEstado", vhEstado));
                cmd.Parameters.Add(new SqlParameter("@idCliente", idCliente));
                await cmd.ExecuteNonQueryAsync();
            }
            int idVehiculo = Convert.ToInt32(await new SqlCommand("SELECT SCOPE_IDENTITY()", (SqlConnection)conn, (SqlTransaction)tran).ExecuteScalarAsync());
            tran.Commit();

            return RedirectToAction("Crear", new { clienteID = idCliente, vehiculoID = idVehiculo });
        }

        [HttpGet]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> SeleccionarClienteOrden(string nombre = null, bool? activo = null)
        {
            var clientes = new List<dynamic>();
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Get_Clientes";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Nombre", (object)nombre ?? DBNull.Value));
                cmd.Parameters.Add(new SqlParameter("@Activo", (object)activo ?? DBNull.Value));
                using var rd = await cmd.ExecuteReaderAsync();
                while (await rd.ReadAsync())
                {
                    dynamic c = new ExpandoObject();
                    c.Id = rd.GetInt32(0);
                    c.Nombre = rd.GetString(1);
                    c.Apellido = rd.GetString(2);
                    c.Telefono = rd.GetString(3);
                    c.Vehiculos = new List<dynamic>();
                    clientes.Add(c);
                }
            }
            foreach (var c in clientes)
            {
                int clientId = (int)c.Id;

                var vehs = await _bd.Vehiculos
                    .Where(v => v.IdCliente == clientId
                                && v.VhActivo == true)    
                    .Select(v => new { v.IdVehiculo, v.VhMatricula, v.VhMarca })
                    .ToListAsync();

                c.Vehiculos = vehs;
            }


            ViewBag.FiltroNombre = nombre;
            ViewBag.FiltroActivo = activo;
            return View(clientes);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> Crear(int clienteID, int vehiculoID, int? presupuestoID = null)
        {
            ViewBag.ClienteID = clienteID;
            ViewBag.VehiculoID = vehiculoID;
            ViewBag.PresupuestoID = presupuestoID;

            ViewBag.Estados = new[] { "Pendiente", "En Proceso", "Entregado" };
            ViewBag.Tipos = new[] { "Mantenimiento", "Reparación", "Inspección" };

            ViewBag.Mecanicos = await _bd.Empleados
                .Where(e => e.EmpActivo && e.IdRolNavigation.RTipo == "Mecánico")
                .Include(e => e.IdPersonaNavigation)
                .Select(e => new
                {
                    e.IdEmpleado,
                    NombreCompleto = e.IdPersonaNavigation.PNombre + " " + e.IdPersonaNavigation.PApellido
                }).ToListAsync();


            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> Crear(
            int clienteID,
            int vehiculoID,
            string orEstado,
            string orTipo,
            DateTime orFechaEntrega,
            DateTime? orFechaFin,
            string orFalla,
            int? presupuestoID,
            int? idMecanicoTemp
        )
        {
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Ins_Orden";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@orEstado", orEstado));
            cmd.Parameters.Add(new SqlParameter("@orTipo", orTipo));
            cmd.Parameters.Add(new SqlParameter("@orFechaEntrega", orFechaEntrega));
            cmd.Parameters.Add(new SqlParameter("@orFechaFin", (object)orFechaFin ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@orFalla", orFalla));
            cmd.Parameters.Add(new SqlParameter("@idCliente", clienteID));
            cmd.Parameters.Add(new SqlParameter("@idVehiculo", vehiculoID));
            cmd.Parameters.Add(new SqlParameter("@idPresupuesto", (object)presupuestoID ?? DBNull.Value));
            await cmd.ExecuteNonQueryAsync();

            if (idMecanicoTemp.HasValue)
                TempData["MecanicoTemp"] = idMecanicoTemp;

            return RedirectToAction("Lista");
        }

        [HttpGet]
        [Authorize(Roles = "Administrador, Cajero, Supervisor, Mecanico")]
        public async Task<IActionResult> Editar(int id)
        {
            var orden = await _bd.Ordenes.FindAsync(id);
            if (orden == null) return NotFound();

            var cliente = await _bd.Clientes
                .Include(c => c.IdPersonaNavigation)
                .FirstAsync(c => c.IdCliente == orden.IdCliente);
            var vehiculo = await _bd.Vehiculos.FindAsync(orden.IdVehiculo);

            ViewBag.ClienteID = cliente.IdCliente;
            ViewBag.VehiculoID = vehiculo.IdVehiculo;
            ViewBag.ClienteNombre = cliente.IdPersonaNavigation.PNombre + " " + cliente.IdPersonaNavigation.PApellido;
            ViewBag.Matricula = vehiculo.VhMatricula;

            ViewBag.Estados = new[] { "Pendiente", "En Proceso", "Entregado", "Finalizado" };
            ViewBag.Tipos = new[] { "Mantenimiento", "Reparación", "Inspección" };
            ViewBag.Packs = await _bd.PackReparaciones.Where(p => p.PckActivo).ToListAsync();
            ViewBag.Productos = await _bd.ProductoRepuestos.Where(p => p.PrActivo).ToListAsync();
            ViewBag.Servicios = await _bd.Servicios.Where(s => s.SerActivo).ToListAsync();

            var detalles = new List<dynamic>();
            // Packs
            var dpacks = await _bd.OrdenDetallePacks
                .Include(d => d.IdPackNavigation)
                .Where(d => d.IdOrden == id)
                .ToListAsync();
            foreach (var d in dpacks)
                detalles.Add(new
                {
                    Tipo = "Pack",
                    Nombre = d.IdPackNavigation.PckNombre,
                    Cantidad = d.OpCantidad,
                    Precio = d.OpPrecioPack,
                    IdDetalle = d.IdDetalle
                });
            // Productos
            var dprods = await _bd.OrdenDetalleProductos
                .Include(d => d.IdProductoNavigation)
                .Where(d => d.IdOrden == id)
                .ToListAsync();
            foreach (var d in dprods)
                detalles.Add(new
                {
                    Tipo = "Producto",
                    Nombre = d.IdProductoNavigation.PrNombre,
                    Cantidad = d.OpCantidad,
                    Precio = d.OpPrecioUnit,
                    IdDetalle = d.IdDetalle
                });
            // Servicios
            var dservs = await _bd.OrdenDetalleServicios
                .Include(d => d.IdServicioNavigation)
                .Where(d => d.IdOrden == id)
                .ToListAsync();
            foreach (var d in dservs)
                detalles.Add(new
                {
                    Tipo = "Servicio",
                    Nombre = d.IdServicioNavigation.SerNombre,
                    Cantidad = d.OpCantidad,
                    Precio = d.OpPrecioUnit,
                    IdDetalle = d.IdDetalle
                });
            ViewBag.Detalles = detalles;

            ViewBag.Mecanicos = await _bd.Empleados
                .Where(e => e.EmpActivo && e.IdRolNavigation.RTipo == "Mecánico")
                .Include(e => e.IdPersonaNavigation)
                .Select(e => new {
                    e.IdEmpleado,
                    NombreCompleto = e.IdPersonaNavigation.PNombre + " " + e.IdPersonaNavigation.PApellido
                })
                .ToListAsync();


            ViewBag.YaPago = await _bd.DetallePagoOrdens
                .AnyAsync(dp => dp.IdOrden == id);

            ViewBag.CostoTotal = orden.OrCostoTotal;


            return View("Crear", orden);
        }


        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> Editar(
    int idOrden,
    string orEstado,
    float orCostoTotal,
    string orTipo,
    DateTime orFechaEntrega,
    DateTime? orFechaFin,
    string orFalla,
    int? idPresupuesto,
    int? idMecanicoTemp
)
        {
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Upd_Orden";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@idOrden", idOrden));
            cmd.Parameters.Add(new SqlParameter("@orEstado", orEstado));
            cmd.Parameters.Add(new SqlParameter("@orCostoTotal", orCostoTotal));
            cmd.Parameters.Add(new SqlParameter("@orTipo", orTipo));
            cmd.Parameters.Add(new SqlParameter("@orFechaEntrega", orFechaEntrega));
            cmd.Parameters.Add(new SqlParameter(
                "@orFechaFin",
                orFechaFin.HasValue ? (object)orFechaFin.Value : DBNull.Value
            ));
            cmd.Parameters.Add(new SqlParameter("@orFalla", orFalla));
            cmd.Parameters.Add(new SqlParameter(
                "@idPresupuesto",
                idPresupuesto.HasValue ? (object)idPresupuesto.Value : DBNull.Value
            ));

            await cmd.ExecuteNonQueryAsync();

            if (idMecanicoTemp.HasValue)
                TempData["MecanicoTemp"] = idMecanicoTemp.Value;

            return RedirectToAction("Lista");
        }


        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> AgregarPack(int idOrden, int idPack, int opCantidad)
        {
            try
            {
                using var conn = _bd.Database.GetDbConnection();
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "Ins_OrdenDetPack";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idOrden", idOrden));
                cmd.Parameters.Add(new SqlParameter("@idPack", idPack));
                cmd.Parameters.Add(new SqlParameter("@opCantidad", opCantidad));
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {

                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Editar", new { id = idOrden });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> AgregarProducto(int idOrden, int idProducto, int opCantidad)
        {
            try
            {
                using var conn = _bd.Database.GetDbConnection();
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "Ins_OrdenDetProducto";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idOrden", idOrden));
                cmd.Parameters.Add(new SqlParameter("@idProducto", idProducto));
                cmd.Parameters.Add(new SqlParameter("@opCantidad", opCantidad));
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {

                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Editar", new { id = idOrden });

        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> AgregarServicio(int idOrden, int idServicio, int opCantidad)
        {
            try
            {
                using var conn = _bd.Database.GetDbConnection();
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "Ins_OrdenDetServicio";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idOrden", idOrden));
                cmd.Parameters.Add(new SqlParameter("@idServicio", idServicio));
                cmd.Parameters.Add(new SqlParameter("@opCantidad", opCantidad));
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {

                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Editar", new { id = idOrden });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> ActualizarPack(int idDetalle, int? opCantidad)
        {
            var idOrden = await _bd.OrdenDetallePacks.Where(d => d.IdDetalle == idDetalle).Select(d => d.IdOrden).FirstAsync();
            try
            {
                using var conn = _bd.Database.GetDbConnection();
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "Upd_OrdenDetPack";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idDetalle", idDetalle));
                cmd.Parameters.Add(new SqlParameter("@opCantidad", (object)opCantidad ?? DBNull.Value));
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Stock insuficiente"))
                    TempData["Error"] = "No hay stock suficiente para uno o más componentes del pack.";
                else
                    TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Editar", new { id = idOrden });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> ActualizarProducto(int idDetalle, int? opCantidad)
        {
            var idOrden = await _bd.OrdenDetalleProductos.Where(d => d.IdDetalle == idDetalle).Select(d => d.IdOrden).FirstAsync();


            try
            {
                using var conn = _bd.Database.GetDbConnection();
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "Upd_OrdenDetProducto";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idDetalle", idDetalle));
                cmd.Parameters.Add(new SqlParameter("@opCantidad", (object)opCantidad ?? DBNull.Value));
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {

                if (ex.Message.Contains("Stock insuficiente"))
                    TempData["Error"] = "No hay suficiente stock disponible para este producto.";
                else
                    TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Editar", new { id = idOrden });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> ActualizarServicio(int idDetalle, int? opCantidad)
        {
            var idOrden = await _bd.OrdenDetalleServicios.Where(d => d.IdDetalle == idDetalle).Select(d => d.IdOrden).FirstAsync();

            try
            {
                using var conn = _bd.Database.GetDbConnection();
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "Upd_OrdenDetServicio";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idDetalle", idDetalle));
                cmd.Parameters.Add(new SqlParameter("@opCantidad", (object)opCantidad ?? DBNull.Value));
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {

                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Editar", new { id = idOrden });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Supervisor")]
        public async Task<IActionResult> EliminarPack(int idDetalle)
        {
            var idOrden = await _bd.OrdenDetallePacks.Where(d => d.IdDetalle == idDetalle).Select(d => d.IdOrden).FirstAsync();
            try
            {
                using var conn = _bd.Database.GetDbConnection();
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "Del_OrdenDetPack";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idDetalle", idDetalle));
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {

                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Editar", new { id = idOrden });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> EliminarProducto(int idDetalle)
        {
            var idOrden = await _bd.OrdenDetalleProductos.Where(d => d.IdDetalle == idDetalle).Select(d => d.IdOrden).FirstAsync();
            try
            {
                using var conn = _bd.Database.GetDbConnection();
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "Del_OrdenDetProducto";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idDetalle", idDetalle));
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {

                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Editar", new { id = idOrden });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> EliminarServicio(int idDetalle)
        {
            var idOrden = await _bd.OrdenDetalleServicios.Where(d => d.IdDetalle == idDetalle).Select(d => d.IdOrden).FirstAsync();
            try
            {
                using var conn = _bd.Database.GetDbConnection();
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "Del_OrdenDetServicio";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idDetalle", idDetalle));
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {

                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Editar", new { id = idOrden });
        }

        #endregion


        #region Pago_Entrega

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor, Mecanico")]
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
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Ins_OrdenDetEntrega";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idOrden", idOrden));
            cmd.Parameters.Add(new SqlParameter("@recibido_por", recibido_por));
            cmd.Parameters.Add(new SqlParameter("@telefono_recibe", telefono_recibe));
            cmd.Parameters.Add(new SqlParameter("@es_propietario", es_propietario));
            cmd.Parameters.Add(new SqlParameter("@entregado_por", entregado_por));
            cmd.Parameters.Add(new SqlParameter("@orden_entrega", orden_entrega));
            cmd.Parameters.Add(new SqlParameter("@mecanico_responsable", mecanico_responsable));
            cmd.Parameters.Add(new SqlParameter("@informe_tecnico", informe_tecnico));
            await cmd.ExecuteNonQueryAsync();

            return RedirectToAction("Editar", new { id = idOrden });
        }


        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor, Mecanico")]
        public async Task<IActionResult> ActualizarEntrega(
            int idEntrega,
            string recibido_por = null,
            string telefono_recibe = null,
            bool? es_propietario = null,
            int? entregado_por = null,
            DateTime? fecha_entrega = null,
            string orden_entrega = null,
            int? mecanico_responsable = null,
            string informe_tecnico = null)
        {
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Upd_OrdenDetEntrega";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idEntrega", idEntrega));
            cmd.Parameters.Add(new SqlParameter("@recibido_por", (object)recibido_por ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@telefono_recibe", (object)telefono_recibe ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@es_propietario", (object)es_propietario ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@entregado_por", (object)entregado_por ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@fecha_entrega", (object)fecha_entrega ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@orden_entrega", (object)orden_entrega ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@mecanico_responsable", (object)mecanico_responsable ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@informe_tecnico", (object)informe_tecnico ?? DBNull.Value));
            await cmd.ExecuteNonQueryAsync();

            var idOrden = await _bd.OrdenDetalleEntregas
                .Where(d => d.IdEntrega == idEntrega)
                .Select(d => d.IdOrden)
                .FirstAsync();

            return RedirectToAction("Editar", new { id = idOrden });
        }



        [HttpGet]
        [Authorize(Roles = "Administrador, Cajero, Supervisor, Mecanico")]
        public async Task<IActionResult> AgregarEntrega(int idOrden)
        {
            var orden = await _bd.Ordenes.FindAsync(idOrden);
            if (orden == null)
            {
                return NotFound();
            }

            var entrega = new OrdenDetalleEntrega
            {
                IdOrden = idOrden
            };

            ViewBag.Empleados = await _bd.Empleados
                .Where(e => e.EmpActivo && e.IdRolNavigation.RTipo == "Mecánico")
                .Include(e => e.IdPersonaNavigation)
                .Select(e => new
                {
                    e.IdEmpleado,
                    NombreCompleto = e.IdPersonaNavigation.PNombre + " " + e.IdPersonaNavigation.PApellido
                })
                .ToListAsync();


            ViewBag.MecanicoAsignado = TempData["MecanicoTemp"];


            return View("EntregaOrden", entrega);
        }







        [HttpGet]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> AgregarPagoOrden(int idOrden)
        {
            var orden = await _bd.Ordenes
            .Include(o => o.DetallePagoOrdens)
            .Include(o => o.IdVehiculoNavigation)
            .FirstOrDefaultAsync(o => o.IdOrden == idOrden);

            if (orden == null)
                return NotFound();

            if (orden.OrCostoTotal == 0 || orden.DetallePagoOrdens.Any())
            {
                TempData["Error"] = orden.OrCostoTotal == 0
                    ? "No puede pagar una orden de costo 0."
                    : "Esta orden ya tiene un pago registrado.";
                return RedirectToAction("Editar", new { id = idOrden });
            }

            int idCliente = orden.IdVehiculoNavigation.IdCliente;
            ViewBag.ClienteID = idCliente;

            var model = new DetallePagoOrden
            {
                IdOrden = idOrden,
                MontoPagado = orden.OrCostoTotal
            };

            var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(claimId, out var idEmpleado))
            {
                TempData["Error"] = "No se pudo obtener la identidad del empleado.";
                return RedirectToAction("Editar", new { id = idOrden });
            }

            var empleado = await _bd.Empleados
                .Include(e => e.IdPersonaNavigation)
                .FirstOrDefaultAsync(e => e.IdEmpleado == idEmpleado);

            ViewBag.EmpleadoNombre = empleado != null
                ? $"{empleado.IdPersonaNavigation.PNombre} {empleado.IdPersonaNavigation.PApellido}"
                : "Empleado no identificado";

            ViewBag.FechaHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            return View(model);
        }








        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> AgregarPagoOrden(
            int idOrden,
            string metodoPago,
            string observaciones,
            int idCliente,
            string orden_entrega
        )
        {
            int idEmpleado = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var orden = await _bd.Ordenes.FindAsync(idOrden);
            if (orden == null)
            {
                TempData["Error"] = "Orden no encontrada.";
                return RedirectToAction("Editar", new { id = idOrden });
            }

            if (orden.OrCostoTotal == 0)
            {
                TempData["Error"] = "No puede realizar pagos en una orden cuyo costo es 0.";
                return RedirectToAction("Editar", new { id = idOrden });
            }

            bool yaPago = await _bd.DetallePagoOrdens.AnyAsync(dp => dp.IdOrden == idOrden);
            if (yaPago)
            {
                TempData["Error"] = "Esta orden ya tiene un pago registrado.";
                return RedirectToAction("Editar", new { id = idOrden });
            }

            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Ins_DetallePagoOrden";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idOrden", idOrden));
            cmd.Parameters.Add(new SqlParameter("@metodoPago", metodoPago));
            cmd.Parameters.Add(new SqlParameter("@observaciones", (object)observaciones ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@idEmpleado", idEmpleado));
            cmd.Parameters.Add(new SqlParameter("@idCliente", idCliente));

            await cmd.ExecuteNonQueryAsync();

            return RedirectToAction("Editar", new { id = idOrden });
        }


        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> ActualizarPagoOrden(
            int idPagoOrden,
            string metodoPago = null,
            string observaciones = null
        )
        {
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Upd_DetallePagoOrden";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idPagoOrden", idPagoOrden));
            cmd.Parameters.Add(new SqlParameter("@metodoPago", (object)metodoPago ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@observaciones", (object)observaciones ?? DBNull.Value));
            await cmd.ExecuteNonQueryAsync();

            var idOrden = await _bd.DetallePagoOrdens
                .Where(d => d.IdPagoOrden == idPagoOrden)
                .Select(d => d.IdOrden)
                .FirstAsync();

            return RedirectToAction("Editar", new { id = idOrden });
        }

        #endregion

        [HttpGet]
        [Authorize(Roles ="Mecanico")]
        public IActionResult AbrirDetalleEntrega() => RedirectToAction("AgregarEntrega", "Orden");
    }
}
