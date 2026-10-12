using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
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
        [Authorize(Roles = "Administrador, Cajero, Supervisor, Mecanico")]
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
                        PrNombre = reader.IsDBNull(1) ? null : reader.GetString(1),
                        PrMarca = reader.IsDBNull(2) ? null : reader.GetString(2),
                        PrCategoria = reader.IsDBNull(3) ? null : reader.GetString(3),
                        PrCosto = reader.IsDBNull(4) ? null : reader.GetDouble(4),
                        PrPrecioVenta = reader.IsDBNull(5) ? null : reader.GetDouble(5),
                        PrStock = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                    });
                }
            }

            foreach (var p in productos)
            {
                p.PrActivo = await ObtenerActivo(p.IdProducto);
            }

            ViewBag.FiltroNombre = nombre;
            ViewBag.FiltroMarca = marca;
            ViewBag.FiltroCategoria = categoria;
            ViewBag.FiltroActivo = activo;

            return View(productos);
        }

        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public IActionResult Crear()
        {
            var marcas = _bdTallerContext.ProductoRepuestos
                         .Where(p => !string.IsNullOrEmpty(p.PrMarca))
                         .Select(p => p.PrMarca)
                         .Distinct()
                         .ToList();

            var categorias = _bdTallerContext.ProductoRepuestos
                                     .Where(p => !string.IsNullOrEmpty(p.PrCategoria))
                                     .Select(p => p.PrCategoria)
                                     .Distinct()
                                     .ToList();

            ViewBag.Marcas = marcas;
            ViewBag.Categorias = categorias;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
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

        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
        public async Task<IActionResult> Editar(int id)
        {
            var prod = await _bdTallerContext.ProductoRepuestos
                          .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (prod == null)
                return NotFound();

            var marcas = await _bdTallerContext.ProductoRepuestos
                             .Where(p => !string.IsNullOrEmpty(p.PrMarca))
                             .Select(p => p.PrMarca)
                             .Distinct()
                             .ToListAsync();

            var categorias = await _bdTallerContext.ProductoRepuestos
                                 .Where(p => !string.IsNullOrEmpty(p.PrCategoria))
                                 .Select(p => p.PrCategoria)
                                 .Distinct()
                                 .ToListAsync();

            ViewBag.Marcas = marcas;
            ViewBag.Categorias = categorias;

            return View(prod);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Cajero, Supervisor")]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Supervisor")]
        public async Task<IActionResult> Toggle(int id)
        {
            await _bdTallerContext.Database.ExecuteSqlRawAsync(
                "EXEC Mod_EstadoProd @idProducto",
                new SqlParameter("@idProducto", id)
            );
            return RedirectToAction(nameof(Lista));
        }
        


        #region Stock y Eliminar Stock
        [HttpGet]
        [Authorize(Roles = "Administrador, Supervisor")]
        public async Task<IActionResult> IncrementarStock(int id)
        {
            var producto = await _bdTallerContext.ProductoRepuestos
                .FirstOrDefaultAsync(p => p.IdProducto == id);
            if (producto == null) return NotFound();

            var proveedores = new List<dynamic>();

            var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Get_ProveedoresPorProducto";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idProducto", id));

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        proveedores.Add(new
                        {
                            IdProveedor = reader.GetInt32(0),
                            PNombre = reader.GetString(1),
                            PrecioReferencia = reader.GetDouble(2)
                        });
                    }
                }
            }

            ViewBag.Proveedores = proveedores;

            var ultimasCompras = new List<dynamic>();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                SELECT TOP 5 
                cp.idCompra, 
                cp.compFecha, 
                cpro.cproCantidad,
                cpro.idProveedor, 
                pr.pNombre         AS ProveedorNombre,
                cp.cprActivo       AS CprActivo
                FROM Compra_Proveedores cp
                JOIN Compra_Producto   cpro ON cpro.idCompra = cp.idCompra
                LEFT JOIN Proveedor     pr   ON pr.idProveedor = cpro.idProveedor
                WHERE cpro.idProducto = @idProducto
                ORDER BY cp.compFecha DESC";
                cmd.Parameters.Clear();
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
                        CprActivo = reader.GetBoolean(5)
                    });
                }
            }

            ViewBag.UltimasCompras = ultimasCompras;

            return View(producto);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Supervisor")]
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



        [HttpGet]
        [Authorize(Roles = "Administrador, Supervisor")]
        public async Task<IActionResult> Listar()
        {
            var compras = await _bdTallerContext.CompraProveedores
                .Where(c => c.CprActivo)
                .ToListAsync();

            return View(compras);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var compra = await _bdTallerContext.CompraProveedores
                .FirstOrDefaultAsync(c => c.IdCompra == id && c.CprActivo);
            if (compra == null) return NotFound();

            return View(compra);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            // 1) Verificar que la compra exista y esté activa
            var compraHeader = await _bdTallerContext.CompraProveedores
                .FirstOrDefaultAsync(c => c.IdCompra == id);

            if (compraHeader == null)
            {
                TempData["ErrorMessage"] = $"La compra {id} no existe.";
                return RedirectToAction(nameof(Lista));
            }
            if (!compraHeader.CprActivo)
            {
                TempData["ErrorMessage"] = $"La compra {id} ya fue devuelta.";
                return RedirectToAction(nameof(Lista));
            }

            // 2) Recoger los detalles: producto y cantidad
            var detalles = await _bdTallerContext.CompraProductos
                .Where(cp => cp.IdCompra == id && cp.CproActivo)
                .Select(cp => new { cp.IdProducto, cp.CproCantidad })
                .ToListAsync();

            // 3) Verificar que haya suficiente stock para restar cada detalle
            var insuficientes = new List<int>();
            foreach (var d in detalles)
            {
                var prod = await _bdTallerContext.ProductoRepuestos
                    .FirstOrDefaultAsync(p => p.IdProducto == d.IdProducto);
                if (prod == null || prod.PrStock < d.CproCantidad)
                {
                    insuficientes.Add(d.IdProducto);
                }
            }
            if (insuficientes.Any())
            {
                TempData["ErrorMessage"] = "No es posible devolver la compra porque " +
                    "no hay suficiente stock para los productos: " +
                    string.Join(", ", insuficientes);
                return RedirectToAction(nameof(IncrementarStock), new { id = detalles.First().IdProducto });
            }

            // 4) Todo OK: llamamos al SP
            try
            {
                await _bdTallerContext.Database.ExecuteSqlRawAsync(
                    "EXEC Devolver_CompraProveedor @idCompra",
                    new SqlParameter("@idCompra", id)
                );

                TempData["SuccessMessage"] = $"Compra {id} devuelta correctamente.";
            }
            catch (SqlException ex)
            {
                TempData["ErrorMessage"] = $"Error al devolver la compra: {ex.Message}";
                return RedirectToAction(nameof(Lista));
            }

            // 5) Redirigir al stock del primer producto afectado
            return RedirectToAction(nameof(IncrementarStock), new { id = detalles.First().IdProducto });
        }



        #endregion


        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ProveedoresProducto(int idProducto)
        {
            var proveedores = await _bdTallerContext.ProveedorProductos
                .Where(pp => pp.IdProducto == idProducto && pp.Activo && pp.IdProveedorNavigation.PActivo)
                .Include(pp => pp.IdProveedorNavigation) // carga el proveedor relacionado
                .AsNoTracking()
                .ToListAsync();

            return View(proveedores);
        }


        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PorProducto(int idProducto)
        {
            var proveedores = new List<Proveedor>();

            var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "Get_ProveedoresPorProducto";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idProducto", idProducto));

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        proveedores.Add(new Proveedor
                        {
                            IdProveedor = reader.GetInt32(0),
                            PNombre = reader.GetString(1),
                            // Otros campos que quieras incluir...
                        });
                    }
                }
            }

            // Aquí puedes:
            // - Devolver una vista con la lista
            // - O devolver JSON para APIs (ejemplo a continuación)

            return Json(proveedores);
        }


        [HttpGet]
        [Authorize(Roles = "Administrador, Supervisor")]
        public async Task<IActionResult> HistorialProducto(int idProducto)
        {
            var conn = _bdTallerContext.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Get_HistorialProducto";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@idProducto", SqlDbType.Int) { Value = idProducto });

            var dt = new DataTable();
            using (var reader = await cmd.ExecuteReaderAsync())
                dt.Load(reader);

            await conn.CloseAsync();

            ViewBag.IdProducto = idProducto;
            return View(dt);    // pasamos el DataTable directamente
        }








    }

    public class IncrementarStockViewModel
    {
        public int IdProducto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Cantidad { get; set; }

        // Opcional: permitir indicar un proveedor
        public int? IdProveedor { get; set; }
    }


    
    

}
