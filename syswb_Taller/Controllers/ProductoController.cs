using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers
{
    public class ProductoController : Controller
    {
        private readonly BdTallerContext _bdTallerContext;

        public ProductoController(BdTallerContext bdTallerContext)
        {
            _bdTallerContext = bdTallerContext;
        }


        private async Task<bool> ObtenerActivo(int idProducto)
        {
            // Creamos una nueva conexión para evitar problemas de MARS
            var connString = _bdTallerContext.Database.GetConnectionString();
            await using var conn2 = new SqlConnection(connString);
            await conn2.OpenAsync();

            await using var cmd2 = new SqlCommand(
                "SELECT prActivo FROM Producto_Repuesto WHERE idProducto = @id", conn2);
            cmd2.Parameters.AddWithValue("@id", idProducto);

            var resultado = await cmd2.ExecuteScalarAsync();
            return resultado != DBNull.Value && (bool)resultado;
        }



        [HttpGet]
        public async Task<IActionResult> Lista(string? nombre, string? marca, string? categoria, bool? activo)
        {
            var productos = new List<ProductoRepuesto>();

            // 1) Obtenemos la lista base con Get_Productos
            await using var conn = _bdTallerContext.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            await using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Get_Productos";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.VarChar, 100) { Value = (object?)nombre ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Marca", SqlDbType.VarChar, 100) { Value = (object?)marca ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Categoria", SqlDbType.VarChar, 100) { Value = (object?)categoria ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = (object?)activo ?? DBNull.Value });

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    productos.Add(new ProductoRepuesto
                    {
                        IdProducto = reader.GetInt32(0),
                        PrNombre = reader.GetString(1),
                        PrMarca = reader.GetString(2),
                        PrCategoria = reader.GetString(3),
                        PrCosto = reader.IsDBNull(4) ? null : reader.GetDouble(4),
                        PrPrecioVenta = reader.IsDBNull(5) ? null : reader.GetDouble(5),
                        PrStock = reader.GetInt32(6),
                        // ¡No pongo PrActivo aquí!
                    });
                }
            }

            // 2) Ahora, para cada producto, preguntamos a la BD su estado real
            foreach (var p in productos)
            {
                p.PrActivo = await ObtenerActivo(p.IdProducto);
            }

            // 3) Pasamos los filtros al ViewBag
            ViewBag.FiltroNombre = nombre;
            ViewBag.FiltroMarca = marca;
            ViewBag.FiltroCategoria = categoria;
            ViewBag.FiltroActivo = activo;

            return View(productos);
        }


        public IActionResult Crear()
        {
            return View();
        }

        // POST: Crear Producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(string prNombre, string prMarca, string prCategoria, double? prCosto, double? prPrecioVenta)
        {
            try
            {
                var pNombre = new SqlParameter("@prNombre", prNombre);
                var pMarca = new SqlParameter("@prMarca", prMarca ?? (object)DBNull.Value);
                var pCategoria = new SqlParameter("@prCategoria", prCategoria ?? (object)DBNull.Value);
                var pCosto = new SqlParameter("@prCosto", prCosto.HasValue ? (object)prCosto.Value : DBNull.Value);
                var pPrecio = new SqlParameter("@prPrecioVenta", prPrecioVenta.HasValue ? (object)prPrecioVenta.Value : DBNull.Value);
                await _bdTallerContext.Database.ExecuteSqlRawAsync(
                    "EXEC Ins_Producto @prNombre,@prMarca,@prCategoria,@prCosto,@prPrecioVenta",
                    pNombre, pMarca, pCategoria, pCosto, pPrecio
                );
                TempData["SuccessMessage"] = "Producto creado correctamente.";
                return RedirectToAction(nameof(Lista));
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: Editar Producto
        public async Task<IActionResult> Editar(int id)
        {
            // Reutilizar Lista para obtener todos
            var listaResult = await Lista(null, null, null, null) as ViewResult;
            var lista = listaResult?.Model as List<ProductoRepuesto> ?? new List<ProductoRepuesto>();
            var prod = lista.FirstOrDefault(p => p.IdProducto == id);
            if (prod == null) return NotFound();
            return View(prod);
        }

        // POST: Editar Producto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, string prNombre, string prMarca, string prCategoria, double? prCosto, double? prPrecioVenta, int? prStock)
        {
            try
            {
                var pId = new SqlParameter("@idProducto", id);
                var pNombre = new SqlParameter("@prNombre", prNombre);
                var pMarca = new SqlParameter("@prMarca", prMarca ?? (object)DBNull.Value);
                var pCategoria = new SqlParameter("@prCategoria", prCategoria ?? (object)DBNull.Value);
                var pCosto = new SqlParameter("@prCosto", prCosto.HasValue ? (object)prCosto.Value : DBNull.Value);
                var pPrecio = new SqlParameter("@prPrecioVenta", prPrecioVenta.HasValue ? (object)prPrecioVenta.Value : DBNull.Value);
                var pStock = new SqlParameter("@prStock", prStock.HasValue ? (object)prStock.Value : DBNull.Value);
                await _bdTallerContext.Database.ExecuteSqlRawAsync(
                    "EXEC Upd_Producto @idProducto,@prNombre,@prMarca,@prCategoria,@prCosto,@prPrecioVenta,@prStock",
                    pId, pNombre, pMarca, pCategoria, pCosto, pPrecio, pStock
                );
                TempData["SuccessMessage"] = "Producto actualizado correctamente.";
                return RedirectToAction(nameof(Lista));
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError("", ex.Message);
                var prod = new ProductoRepuesto
                {
                    IdProducto = id,
                    PrNombre = prNombre,
                    PrMarca = prMarca,
                    PrCategoria = prCategoria,
                    PrCosto = prCosto,
                    PrPrecioVenta = prPrecioVenta,
                    PrStock = prStock ?? 0
                };
                return View(prod);
            }
        }

        // POST: Alternar estado activo/inactivo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            await _bdTallerContext.Database.ExecuteSqlRawAsync(
                "EXEC Mod_EstadoProd @idProducto",
                new SqlParameter("@idProducto", id)
            );
            return RedirectToAction(nameof(Lista));
        }


        // Dentro de ProductoController.cs
        [HttpGet]
        public async Task<IActionResult> IncrementarStock(int id)
        {
            // 1) Recuperar el producto
            var producto = await _bdTallerContext.ProductoRepuestos
                .FirstOrDefaultAsync(p => p.IdProducto == id);
            if (producto == null) return NotFound();

            // 2) Cargar proveedores en ViewBag.Proveedores (ya lo tienes)

            // 3) Cargar las últimas compras de este producto
            //    Vamos a traer las 5 compras más recientes:
            var ultimasCompras = new List<dynamic>();
            var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            // … dentro de la acción IncrementarStock(GET) …

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
    SELECT TOP 5 
      cp.idCompra, 
      cp.compFecha, 
      cpro.cproCantidad,
      cpro.idProveedor, 
      pr.pNombre         AS ProveedorNombre,
      cp.cprActivo       AS CprActivo      -- <--- lo agregamos aquí
    FROM Compra_Proveedores cp
    JOIN Compra_Producto   cpro ON cpro.idCompra = cp.idCompra
    LEFT JOIN Proveedor     pr   ON pr.idProveedor = cpro.idProveedor
    WHERE cpro.idProducto = @idProducto
    ORDER BY cp.compFecha DESC";
                cmd.Parameters.Add(new SqlParameter("@idProducto", id));

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    ultimasCompras.Add(new
                    {
                        IdCompra = reader.GetInt32(0),
                        CompFecha = reader.GetDateTime(1).ToString("g"),
                        CproCantidad = reader.GetInt32(2),
                        IdProveedor = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                        ProveedorNombre = reader.IsDBNull(4) ? null : reader.GetString(4),
                        CprActivo = reader.GetBoolean(5)    // <-- y lo leemos aquí
                    });
                }
            }


            ViewBag.UltimasCompras = ultimasCompras;

            // 4) Devolver la vista con el modelo ProductoRepuesto
            return View(producto);
        }




        // POST: Ejecutar SP Inc_StockProducto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IncrementarStock(int id, int? IdProveedor, int cantidad)
        {
            var pId = new SqlParameter("@idProducto", id);
            var pCantidad = new SqlParameter("@cantidad", cantidad);
            var pProveedor = new SqlParameter("@idProveedor", (object?)IdProveedor ?? DBNull.Value);

            await _bdTallerContext.Database.ExecuteSqlRawAsync(
                "EXEC Inc_StockProducto @idProducto,@cantidad,@idProveedor",
                pId, pCantidad, pProveedor
            );

            return RedirectToAction(nameof(Lista));
        }



        //Eliminar
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            // Suponiendo que tienes un DbSet<Compra_Proveedores> en tu context
            var compras = await _bdTallerContext.CompraProveedores
                .Where(c => c.CprActivo)
                .ToListAsync();

            return View(compras);
        }

        // GET: CompraProveedor/Eliminar/5
        // Muestra confirmación antes de borrar
        // GET: Producto/Eliminar/5
        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var compra = await _bdTallerContext.CompraProveedores
                .FirstOrDefaultAsync(c => c.IdCompra == id && c.CprActivo);
            if (compra == null) return NotFound();

            return View(compra);
        }

        // POST: Producto/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                await _bdTallerContext.Database.ExecuteSqlRawAsync(
                    "EXEC Del_CompraProveedor @idCompra",
                    new SqlParameter("@idCompra", id)
                );

                TempData["SuccessMessage"] = $"Compra {id} anulada correctamente.";
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Lista"); // Esto vuelve al listado de compras
        }

    }

    // ViewModel para la vista de incremento de stock
    public class IncrementarStockViewModel
    {
        public int IdProducto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Cantidad { get; set; }

        // Opcional: permitir indicar un proveedor
        public int? IdProveedor { get; set; }
    }


    //Eliminar Stock
    

}
