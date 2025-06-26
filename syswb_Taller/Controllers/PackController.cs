using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Dynamic;
using System.Linq;
using syswb_Taller.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace syswb_Taller.Controllers
{
    public class PackController : Controller
    {
        private readonly BdTallerContext _bdTallerContext;

        public PackController(BdTallerContext bdTallerContext)
        {
            _bdTallerContext = bdTallerContext;
        }

        #region CRUD Pack

        [HttpGet]
        [Authorize(Roles = "Administrador, Cajero, Supervisor, Mecanico")]
        public async Task<IActionResult> Lista(string nombre = null, bool? activo = null)
        {
            var packs = new List<dynamic>();
            // obtenemos la conexión relacional
            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Get_Packs";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@Nombre", (object)nombre ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Activo", (object)activo ?? DBNull.Value));

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                packs.Add(new
                {
                    IdPack = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Precio = !reader.IsDBNull(2) ? reader.GetDouble(2) : 0.0,
                    Activo = reader.GetBoolean(3)
                });
            }

            ViewBag.FiltroNombre = nombre;
            ViewBag.FiltroActivo = activo;
            ViewBag.Packs = packs;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> Crear()
        {
            // Productos y servicios para dropdowns
            ViewBag.Productos = await _bdTallerContext.ProductoRepuestos
                                      .Where(p => p.PrActivo)
                                      .AsNoTracking()
                                      .ToListAsync();
            ViewBag.Servicios = await _bdTallerContext.Servicios
                                      .Where(s => s.SerActivo)
                                      .AsNoTracking()
                                      .ToListAsync();

            // Inicializo detalles vacíos y costo cero
            ViewBag.Detalles = new List<dynamic>();
            ViewBag.CostoTotal = 0.0;
            ViewBag.IdPack = 0;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> Crear(string pckNombre, string pckDescripcion)
        {
            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Ins_PackReparacion";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@pckNombre", pckNombre));
            cmd.Parameters.Add(new SqlParameter("@pckDescripcion",
                                (object)pckDescripcion ?? DBNull.Value));

            var outParam = new SqlParameter("@NewPackId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outParam);

            await cmd.ExecuteNonQueryAsync();

            if (outParam.Value == DBNull.Value)
            {
                TempData["Error"] = "No fue posible crear el pack.";
                return RedirectToAction("Lista");
            }

            int newPackId = (int)outParam.Value;
            return RedirectToAction("Editar", new { id = newPackId });
        }


        [HttpGet]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> Editar(int id)
        {
            // Pack principal
            var packEntity = await _bdTallerContext.PackReparaciones.FindAsync(id);
            if (packEntity == null) return NotFound();
            ViewBag.Pack = new
            {
                IdPack = packEntity.IdPack,
                PckNombre = packEntity.PckNombre,
                PckDescripcion = packEntity.PckDescripcion
            };
            ViewBag.IdPack = id;

            // Dropdowns de productos y servicios
            ViewBag.Productos = await _bdTallerContext.ProductoRepuestos
                                      .Where(p => p.PrActivo)
                                      .AsNoTracking()
                                      .ToListAsync();
            ViewBag.Servicios = await _bdTallerContext.Servicios
                                      .Where(s => s.SerActivo)
                                      .AsNoTracking()
                                      .ToListAsync();

            // Cargo detalles mediante el SP y sumo costo
            var detalles = new List<dynamic>();
            double costo = 0;

            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Get_DetallePack";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idPack", id));

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                string tipo = reader.GetString(0);
                int idDet = reader.GetInt32(1);
                string nombre = reader.GetString(2);
                int cantidad = reader.GetInt32(3);
                double precio = reader.GetDouble(4);

                // Construyo un ExpandoObject para que Razor vea bien las propiedades
                dynamic det = new ExpandoObject();
                det.Tipo = tipo;
                det.Id = idDet;
                det.Nombre = nombre;
                det.Cantidad = cantidad;
                det.Precio = precio;

                detalles.Add(det);
                costo += cantidad * precio;
            }

            ViewBag.Detalles = detalles;
            ViewBag.CostoTotal = costo;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> Editar(int idPack, string pckNombre, string pckDescripcion)
        {
            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Upd_PackReparacion";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idPack", idPack));
            cmd.Parameters.Add(new SqlParameter("@pckNombre", pckNombre));
            cmd.Parameters.Add(new SqlParameter("@pckDescripcion", (object)pckDescripcion ?? DBNull.Value));
            await cmd.ExecuteNonQueryAsync();
            return RedirectToAction("Lista");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Supervisor")]
        public async Task<IActionResult> CambiarEstado(int idPack)
        {
            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Mod_EstadoPack";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idPack", idPack));
            await cmd.ExecuteNonQueryAsync();
            return RedirectToAction("Lista");
        }

        #endregion

        #region CRUD Detalles

        [HttpGet]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> Detalles(int idPack)
        {
            // Carga el pack
            var pack = await _bdTallerContext.PackReparaciones.FindAsync(idPack);
            if (pack == null) return NotFound();
            ViewBag.Pack = pack;

            // Productos y servicios para buscar
            ViewBag.Productos = await _bdTallerContext.ProductoRepuestos
                                        .Where(p => p.PrActivo)
                                        .AsNoTracking()
                                        .ToListAsync();
            ViewBag.Servicios = await _bdTallerContext.Servicios
                                        .Where(s => s.SerActivo)
                                        .AsNoTracking()
                                        .ToListAsync();

            // Detalles existentes + costo total
            var detalles = new List<dynamic>();
            double costo = 0;
            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Get_DetallePack";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idPack", idPack));

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var tipo = reader.GetString(0);
                var idDet = reader.GetInt32(1);
                var nombre = reader.GetString(2);
                var cantidad = reader.GetInt32(3);
                var precio = reader.GetDouble(4);
                detalles.Add(new { Tipo = tipo, Id = idDet, Nombre = nombre, Cantidad = cantidad, Precio = precio });
                costo += cantidad * precio;
            }

            ViewBag.Detalles = detalles;
            ViewBag.CostoTotal = costo;
            ViewBag.IdPack = idPack;
            return View("Editar");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> AgregarProducto(int idPack, int idProducto, int pdCantidad = 1)
        {
            try
            {
                using var conn = _bdTallerContext.Database.GetDbConnection();
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "Ins_DetallePackProducto";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idPack", idPack));
                cmd.Parameters.Add(new SqlParameter("@idProducto", idProducto));
                cmd.Parameters.Add(new SqlParameter("@pdCantidad", pdCantidad));
                await cmd.ExecuteNonQueryAsync();

                return RedirectToAction("Detalles", new { idPack });
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("ya está asignado al pack"))  
                {
                    TempData["Error"] = "El producto ya está asignado a este pack.";
                    return RedirectToAction("Detalles", new { idPack });
                }
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> AgregarServicio(int idPack, int idServicio, int psCantidad = 1)
        {
            try
            {
                using var conn = _bdTallerContext.Database.GetDbConnection();
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "Ins_DetallePackServicio";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idPack", idPack));
                cmd.Parameters.Add(new SqlParameter("@idServicio", idServicio));
                cmd.Parameters.Add(new SqlParameter("@psCantidad", psCantidad));
                await cmd.ExecuteNonQueryAsync();

                return RedirectToAction("Detalles", new { idPack });
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("ya está asignado al pack"))  
                {
                    TempData["Error"] = "El servicio ya está asignado a este pack.";
                    return RedirectToAction("Detalles", new { idPack });
                }
                throw;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> ActualizarProducto(int idDetalle, int? pdCantidad)
        {
            var idPack = await _bdTallerContext.DetallePackProductos
                                .Where(d => d.IdDetalle == idDetalle)
                                .Select(d => d.IdPack)
                                .FirstAsync();

            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Upd_DetallePackProducto";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idDetalle", idDetalle));
            cmd.Parameters.Add(new SqlParameter("@pdCantidad", (object)pdCantidad ?? DBNull.Value));
            await cmd.ExecuteNonQueryAsync();
            return RedirectToAction("Detalles", new { idPack });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> ActualizarServicio(int idDetalle, int? psCantidad)
        {
            var idPack = await _bdTallerContext.DetallePackServicios
                                .Where(d => d.IdDetalle == idDetalle)
                                .Select(d => d.IdPack)
                                .FirstAsync();

            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Upd_DetallePackServicio";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idDetalle", idDetalle));
            cmd.Parameters.Add(new SqlParameter("@psCantidad", (object)psCantidad ?? DBNull.Value));
            await cmd.ExecuteNonQueryAsync();
            return RedirectToAction("Detalles", new { idPack });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> EliminarProducto(int idDetalle)
        {
            var idPack = await _bdTallerContext.DetallePackProductos
                                .Where(d => d.IdDetalle == idDetalle)
                                .Select(d => d.IdPack)
                                .FirstAsync();

            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Del_DetallePackProducto";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idDetalle", idDetalle));
            await cmd.ExecuteNonQueryAsync();
            return RedirectToAction("Detalles", new { idPack });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> EliminarServicio(int idDetalle)
        {
            var idPack = await _bdTallerContext.DetallePackServicios
                                .Where(d => d.IdDetalle == idDetalle)
                                .Select(d => d.IdPack)
                                .FirstAsync();

            using var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Del_DetallePackServicio";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idDetalle", idDetalle));
            await cmd.ExecuteNonQueryAsync();
            return RedirectToAction("Detalles", new { idPack });
        }

        #endregion
    }
}
