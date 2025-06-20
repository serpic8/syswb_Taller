using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Dynamic;
using System.Security.Cryptography;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers
{
    public class PresupuestoController : Controller
    {
        private readonly BdTallerContext _bd;

        public PresupuestoController(BdTallerContext bd) => _bd = bd;

        #region CRUD_Presupuesto

        [HttpGet]
        public async Task<IActionResult> Lista(DateTime? fechaDesde = null, DateTime? fechaHasta = null, int? clienteID = null, bool? estado = null)
        {
            var list = new List<dynamic>();
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Get_Presupuestos";
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
                    Fecha = rd.GetDateTime(1),
                    Cliente = rd.GetString(2),
                    Estado = rd.GetBoolean(3),
                    Costo = rd.GetDouble(4)
                });
            }

            ViewBag.FiltroFechaDesde = fechaDesde;
            ViewBag.FiltroFechaHasta = fechaHasta;
            ViewBag.FiltroCliente = clienteID;
            ViewBag.FiltroEstado = estado;
            return View(list);
        }

        [HttpGet]
        public IActionResult CrearClienteVehiculo()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
            string vhEstado
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

            int idCliente = (int)await new SqlCommand("SELECT SCOPE_IDENTITY()", (SqlConnection)conn, (SqlTransaction)tran).ExecuteScalarAsync();

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

            int idVehiculo = (int)await new SqlCommand("SELECT SCOPE_IDENTITY()", (SqlConnection)conn, (SqlTransaction)tran).ExecuteScalarAsync();
            tran.Commit();

            return RedirectToAction("CrearPresupuesto", new { clienteID = idCliente, idVehiculo = idVehiculo });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarPresupuesto(int idPresupuesto, string preFalla)
        {
            // 1) ejecuta el UPDATE
            var presupuesto = await _bd.Presupuestos.FindAsync(idPresupuesto);
            if (presupuesto == null) return NotFound();

            presupuesto.PreFalla = preFalla;
            // (si quieres permitir cambiar otros campos, inclúyelos aquí)
            await _bd.SaveChangesAsync();

            // 2) Redirige al listado
            return RedirectToAction("Lista");
        }


        [HttpGet]
        public async Task<IActionResult> EditarPresupuesto(int id)
        {
            // Obtener presupuesto
            var presupuesto = await _bd.Presupuestos.FindAsync(id);
            if (presupuesto == null) return NotFound();

            var cliente = await _bd.Clientes
                .Include(c => c.IdPersonaNavigation)
                .FirstOrDefaultAsync(c => c.IdCliente == presupuesto.IdCliente);

            var vehiculo = await _bd.Vehiculos.FindAsync(presupuesto.IdVehiculo);

            ViewBag.ClienteID = cliente?.IdCliente;
            ViewBag.IdVehiculo = vehiculo?.IdVehiculo;
            ViewBag.ClienteNombre = cliente?.IdPersonaNavigation.PNombre + " " + cliente?.IdPersonaNavigation.PApellido;
            ViewBag.Matricula = vehiculo?.VhMatricula;

            // Catálogos
            ViewBag.Packs = await _bd.PackReparaciones.Where(p => p.PckActivo).ToListAsync();
            ViewBag.Productos = await _bd.ProductoRepuestos.Where(p => p.PrActivo).ToListAsync();
            ViewBag.Servicios = await _bd.Servicios.Where(s => s.SerActivo).ToListAsync();

            // Detalles (packs, productos, servicios)
            var detalles = new List<dynamic>();

            var dPacks = await _bd.PresupuestoDetallePacks
                .Where(d => d.IdPresupuesto == id)
                .Include(d => d.IdPackNavigation)
                .ToListAsync();

            foreach (var dp in dPacks)
            {
                detalles.Add(new
                {
                    Tipo = "Pack",
                    Nombre = dp.IdPackNavigation.PckNombre,
                    Cantidad = dp.PpCantidad,
                    PrecioPack = dp.IdPackNavigation.PckPrecio,
                    IdDetalle = dp.IdDetalle
                });
            }

            var dProductos = await _bd.PresupuestoDetalleProductos
                .Where(d => d.IdPresupuesto == id)
                .Include(d => d.IdProductoNavigation)
                .ToListAsync();

            foreach (var pr in dProductos)
            {
                detalles.Add(new
                {
                    Tipo = "Producto",
                    Nombre = pr.IdProductoNavigation.PrNombre,
                    Cantidad = pr.PpCantidad,
                    PrecioUnit = pr.IdProductoNavigation.PrPrecioVenta,
                    IdDetalle = pr.IdDetalle
                });
            }

            var dServicios = await _bd.PresupuestoDetalleServicios
                .Where(d => d.IdPresupuesto == id)
                .Include(d => d.IdServicioNavigation)
                .ToListAsync();

            foreach (var s in dServicios)
            {
                detalles.Add(new
                {
                    Tipo = "Servicio",
                    Nombre = s.IdServicioNavigation.SerNombre,
                    Cantidad = s.PsCantidad,
                    PrecioUnit = s.IdServicioNavigation.SerPrecioUnit,
                    IdDetalle = s.IdDetalle
                });
            }

            ViewBag.Detalles = detalles;

            return View("Crear", new
            {
                IdPresupuesto = presupuesto.IdPresupuesto,
                Fecha = presupuesto.PreFechaCreacion,
                CostoTotal = presupuesto.PreCostoTotal,
                PreFalla = presupuesto.PreFalla,
                Detalles = detalles
            });

        }



        [HttpGet]
        public async Task<IActionResult> SeleccionarClientePresupuesto(string nombre = null, bool? activo = null)
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

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    dynamic c = new ExpandoObject();
                    c.Id = reader.GetInt32(0);
                    c.Nombre = reader.GetString(1);
                    c.Apellido = reader.GetString(2);
                    c.Telefono = reader.GetString(3);
                    c.Vehiculos = new List<dynamic>();
                    clientes.Add(c);
                }
            }

            foreach (var c in clientes)
            {
                int clientId = (int)c.Id;
                var vehs = await _bd.Vehiculos
                    .Where(v => v.IdCliente == clientId && v.VhActivo == true)
                    .Select(v => new { v.IdVehiculo, v.VhMatricula, v.VhMarca })
                    .ToListAsync();
                c.Vehiculos = vehs;
            }

            ViewBag.FiltroNombre = nombre;
            ViewBag.FiltroActivo = activo;
            return View(clientes);
        }

        // ——————————————————————————
        // 4) CREAR PRESUPUESTO (paso final)
        // ——————————————————————————
        [HttpGet]
        public async Task<IActionResult> CrearPresupuesto(int clienteID, int idVehiculo)
        {
            ViewBag.ClienteID = clienteID;
            ViewBag.IdVehiculo = idVehiculo;

            // Obtienes nombre de cliente y matrícula OK...
            var cliente = await _bd.Clientes
                .Include(c => c.IdPersonaNavigation)
                .FirstOrDefaultAsync(c => c.IdCliente == clienteID);
            ViewBag.ClienteNombre = cliente?.IdPersonaNavigation.PNombre
                                  + " " + cliente?.IdPersonaNavigation.PApellido;

            var veh = await _bd.Vehiculos.FindAsync(idVehiculo);
            ViewBag.Matricula = veh?.VhMatricula;

            // ——— Aquí faltaba poblar los catálogos ———
            ViewBag.Packs = await _bd.PackReparaciones
                                         .Where(p => p.PckActivo)
                                         .ToListAsync();
            ViewBag.Productos = await _bd.ProductoRepuestos
                                         .Where(pr => pr.PrActivo)
                                         .ToListAsync();
            ViewBag.Servicios = await _bd.Servicios
                                         .Where(s => s.SerActivo)
                                         .ToListAsync();

            // en creación no hay detalles todavía
            ViewBag.Detalles = new List<dynamic>();

            return View("Crear");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearPresupuesto(int clienteID, int idVehiculo, string preFalla)
        {
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();

            int idPresupuesto = 0;

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Ins_Presupuesto";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@preFalla", preFalla));
            cmd.Parameters.Add(new SqlParameter("@idVehiculo", idVehiculo));
            cmd.Parameters.Add(new SqlParameter("@idCliente", clienteID));

            // Ejecutar y obtener el ID retornado por el SP
            var result = await cmd.ExecuteScalarAsync();

            if (result != null && result != DBNull.Value)
            {
                idPresupuesto = Convert.ToInt32(result);
            }
            else
            {
                // Manejo de error si no se obtiene ID
                TempData["Error"] = "No se pudo obtener el ID del presupuesto insertado.";
                return RedirectToAction("Lista");
            }

            return RedirectToAction("EditarPresupuesto", new { id = idPresupuesto });
        }

        #endregion

        // INSERCIÓN DE DETALLES

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarPack(int idPresupuesto, int idPack, int ppCantidad)
        {
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Ins_PresupuestoDetPack";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idPresupuesto", idPresupuesto));
            cmd.Parameters.Add(new SqlParameter("@idPack", idPack));
            cmd.Parameters.Add(new SqlParameter("@ppCantidad", ppCantidad));
            await cmd.ExecuteNonQueryAsync();
            return RedirectToAction("EditarPresupuesto", new { id = idPresupuesto });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarProducto(int idPresupuesto, int idProducto, int ppCantidad)
        {
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Ins_PresupuestoDetProducto";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idPresupuesto", idPresupuesto));
            cmd.Parameters.Add(new SqlParameter("@idProducto", idProducto));
            cmd.Parameters.Add(new SqlParameter("@ppCantidad", ppCantidad));
            await cmd.ExecuteNonQueryAsync();
            return RedirectToAction("EditarPresupuesto", new { id = idPresupuesto });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarServicio(int idPresupuesto, int idServicio, int psCantidad)
        {
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Ins_PresupuestoDetServicio";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idPresupuesto", idPresupuesto));
            cmd.Parameters.Add(new SqlParameter("@idServicio", idServicio));
            cmd.Parameters.Add(new SqlParameter("@psCantidad", psCantidad));
            await cmd.ExecuteNonQueryAsync();
            return RedirectToAction("EditarPresupuesto", new { id = idPresupuesto });
        }

        // ACTUALIZACIÓN DE DETALLES

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarPack(int idDetalle, int? ppCantidad)
        {
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Upd_PresupuestoDetPack";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idDetalle", idDetalle));
            cmd.Parameters.Add(new SqlParameter("@ppCantidad", (object)ppCantidad ?? DBNull.Value));
            await cmd.ExecuteNonQueryAsync();

            // Recupera idPresupuesto para redirigir
            int idPresupuesto = await _bd.PresupuestoDetallePacks
                .Where(d => d.IdDetalle == idDetalle)
                .Select(d => d.IdPresupuesto)
                .FirstAsync();

            return RedirectToAction("EditarPresupuesto", new { id = idPresupuesto });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarProducto(int idDetalle, int? ppCantidad)
        {
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Upd_PresupuestoDetProducto";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idDetalle", idDetalle));
            cmd.Parameters.Add(new SqlParameter("@ppCantidad", (object)ppCantidad ?? DBNull.Value));
            await cmd.ExecuteNonQueryAsync();

            int idPresupuesto = await _bd.PresupuestoDetalleProductos
                .Where(d => d.IdDetalle == idDetalle)
                .Select(d => d.IdPresupuesto)
                .FirstAsync();

            return RedirectToAction("EditarPresupuesto", new { id = idPresupuesto });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarServicio(int idDetalle, int? psCantidad)
        {
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Upd_PresupuestoDetServicio";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idDetalle", idDetalle));
            cmd.Parameters.Add(new SqlParameter("@psCantidad", (object)psCantidad ?? DBNull.Value));
            await cmd.ExecuteNonQueryAsync();

            int idPresupuesto = await _bd.PresupuestoDetalleServicios
                .Where(d => d.IdDetalle == idDetalle)
                .Select(d => d.IdPresupuesto)
                .FirstAsync();

            return RedirectToAction("EditarPresupuesto", new { id = idPresupuesto });
        }

        // Dentro de PresupuestoController:

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPack(int idDetalle)
        {
            // 1) Obtén el idPresupuesto antes de borrar
            int idPresupuesto = await _bd.PresupuestoDetallePacks
                .Where(d => d.IdDetalle == idDetalle)
                .Select(d => d.IdPresupuesto)
                .FirstAsync();

            // 2) Ejecuta el SP de borrado
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Del_PresupuestoDetPack";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idDetalle", idDetalle));
            await cmd.ExecuteNonQueryAsync();

            // 3) Redirige de vuelta al editar presupuesto
            return RedirectToAction("EditarPresupuesto", new { id = idPresupuesto });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarProducto(int idDetalle)
        {
            int idPresupuesto = await _bd.PresupuestoDetalleProductos
                .Where(d => d.IdDetalle == idDetalle)
                .Select(d => d.IdPresupuesto)
                .FirstAsync();

            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Del_PresupuestoDetProducto";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idDetalle", idDetalle));
            await cmd.ExecuteNonQueryAsync();

            return RedirectToAction("EditarPresupuesto", new { id = idPresupuesto });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarServicio(int idDetalle)
        {
            int idPresupuesto = await _bd.PresupuestoDetalleServicios
                .Where(d => d.IdDetalle == idDetalle)
                .Select(d => d.IdPresupuesto)
                .FirstAsync();

            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Del_PresupuestoDetServicio";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idDetalle", idDetalle));
            await cmd.ExecuteNonQueryAsync();

            return RedirectToAction("EditarPresupuesto", new { id = idPresupuesto });
        }


    }
}
