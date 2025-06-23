using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers
{
    public class ProveedorController : Controller
    {
        private readonly BdTallerContext _bdTallerContext;

        public ProveedorController(BdTallerContext bdTallerContext)
        {
            _bdTallerContext = bdTallerContext;
        }

        // GET: Lista de proveedores con filtros
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Lista(string nombre, bool? activo)
        {
            var nombreParam = new SqlParameter("@Nombre", (object?)nombre ?? DBNull.Value);
            var activoParam = new SqlParameter("@Activo", (object?)activo ?? DBNull.Value);

            var proveedores = await _bdTallerContext.Proveedores
                .FromSqlRaw("EXEC Get_Proveedores @Nombre, @Activo", nombreParam, activoParam)
                .ToListAsync();

            ViewBag.NombreFiltro = nombre;
            ViewBag.ActivoFiltro = activo;
            return View(proveedores);
        }

        // GET: Mostrar formulario de creación
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public IActionResult Detalle()
        {
            return View();
        }

        // POST: Crear nuevo proveedor
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Detalle(string pNombre, string pTelefono)
        {
            try
            {
                var nombreParam = new SqlParameter("@pNombre", pNombre);
                var telefonoParam = new SqlParameter("@pTelefono", pTelefono);

                await _bdTallerContext.Database.ExecuteSqlRawAsync(
                    "EXEC Ins_Proveedor @pNombre, @pTelefono",
                    nombreParam, telefonoParam
                );

                TempData["SuccessMessage"] = "Proveedor agregado correctamente.";
                return RedirectToAction("Lista");
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: Mostrar formulario de edición
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Editar(int id)
        {
            var proveedor = await _bdTallerContext.Proveedores
                .FirstOrDefaultAsync(p => p.IdProveedor == id);
            if (proveedor == null) return NotFound();

            await CargarDatosProveedor(id);
            return View(proveedor);
        }

        // POST: Guardar cambios de edición
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Editar(int id, string pNombre, string pTelefono)
        {
            try
            {
                var idParam = new SqlParameter("@idProveedor", id);
                var nombreParam = new SqlParameter("@pNombre", pNombre);
                var telefonoParam = new SqlParameter("@pTelefono", pTelefono);

                await _bdTallerContext.Database.ExecuteSqlRawAsync(
                    "EXEC Upd_Proveedor @idProveedor, @pNombre, @pTelefono",
                    idParam, nombreParam, telefonoParam
                );

                TempData["SuccessMessage"] = "Proveedor actualizado correctamente.";
                return RedirectToAction("Lista");
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Recuperar proveedor y recargar ViewBags
                var proveedor = await _bdTallerContext.Proveedores
                    .FirstOrDefaultAsync(p => p.IdProveedor == id);
                await CargarDatosProveedor(id);
                return View(proveedor);
            }
        }

        // GET: Productos asociados como JSON (opcional)
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Productos(int idProveedor)
        {
            var param = new SqlParameter("@idProveedor", idProveedor);
            var lista = await _bdTallerContext.Set<ProveedorProducto>()
                .FromSqlRaw("EXEC Get_ProductosPorProveedor @idProveedor", param)
                .ToListAsync();
            return Json(lista);
        }

        // POST: Agregar producto asociado
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AgregarProducto(int idProveedor, int idProducto, double precioRef)
        {
            var pProv = new SqlParameter("@idProveedor", idProveedor);
            var pProd = new SqlParameter("@idProducto", idProducto);
            var pPrecio = new SqlParameter("@precioRef", precioRef);

            try
            {
                await _bdTallerContext.Database.ExecuteSqlRawAsync(
                    "EXEC Ins_ProveedorProducto @idProveedor, @idProducto, @precioRef",
                    pProv, pProd, pPrecio);

                return RedirectToAction("Editar", new { id = idProveedor });
            }
            catch (SqlException ex)
            {
                TempData["ErrorProducto"] = ex.Message;
                return RedirectToAction("Editar", new { id = idProveedor });
            }
        }

        // POST: Actualizar precio/estado producto asociado
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ActualizarProducto(int idProveedor, int idProducto, double? precioRef, bool? activo)
        {
            var pProv = new SqlParameter("@idProveedor", idProveedor);
            var pProd = new SqlParameter("@idProducto", idProducto);
            var pPrecio = new SqlParameter("@precioRef", precioRef ?? (object)DBNull.Value);
            var pActivo = new SqlParameter("@activo", activo ?? (object)DBNull.Value);

            try
            {
                await _bdTallerContext.Database.ExecuteSqlRawAsync(
                    "EXEC Upd_ProveedorProducto @idProveedor, @idProducto, @precioRef, @activo",
                    pProv, pProd, pPrecio, pActivo);

                return RedirectToAction("Editar", new { id = idProveedor });
            }
            catch (SqlException ex)
            {
                TempData["ErrorProducto"] = ex.Message;
                return RedirectToAction("Editar", new { id = idProveedor });
            }
        }

        // POST: Alternar estado activo del producto asociado
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ToggleProducto(int idProveedor, int idProducto)
        {
            var pProv = new SqlParameter("@idProveedor", idProveedor);
            var pProd = new SqlParameter("@idProducto", idProducto);
            try
            {
                await _bdTallerContext.Database.ExecuteSqlRawAsync(
                    "EXEC Toggle_ProveedorProducto @idProveedor, @idProducto",
                    pProv, pProd);

                return RedirectToAction("Editar", new { id = idProveedor });
            }
            catch (SqlException ex)
            {
                TempData["ErrorProducto"] = ex.Message;
                return RedirectToAction("Editar", new { id = idProveedor });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ToggleProveedor(int idProveedor)
        {
            var pId = new SqlParameter("@idProveedor", idProveedor);
            try
            {
                await _bdTallerContext.Database.ExecuteSqlRawAsync(
                    "EXEC Mod_EstadoProv @idProveedor", pId);
                return RedirectToAction("Lista");
            }
            catch (SqlException ex)
            {
                TempData["ErrorProveedor"] = ex.Message;
                return RedirectToAction("Lista");
            }
        }


        /// <summary>
        /// Método privado para reconstruir productos activos y asociados
        /// </summary>
        private async Task CargarDatosProveedor(int id)
        {
            ViewBag.TodosProductos = await _bdTallerContext.ProductoRepuestos
                .Where(p => p.PrActivo)
                .ToListAsync();

            var asociados = new List<ProveedorProducto>();
            var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "dbo.Get_ProductosPorProveedor";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idProveedor", id));
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                asociados.Add(new ProveedorProducto
                {
                    IdProveedor = id,
                    IdProducto = reader.GetInt32(0),
                    PrecioRef = reader.GetDouble(2),
                    Activo = reader.GetBoolean(3),
                    IdProductoNavigation = new ProductoRepuesto
                    {
                        IdProducto = reader.GetInt32(0),
                        PrNombre = reader.GetString(1)
                    }
                });
            }
            ViewBag.ProductosAsociados = asociados;
        }

    }
}
