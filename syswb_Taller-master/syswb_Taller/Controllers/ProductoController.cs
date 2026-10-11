using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using syswb_Taller.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers;

public class ProductoController : Controller
{
    private readonly BdTallerContext _bdTallerContext;

    public ProductoController(BdTallerContext bdTallerContext)
    {
        _bdTallerContext = bdTallerContext;
    }

    #region Lista y Crear

    [HttpGet]
    [Authorize(Roles = RolesSistema.Todos)]
    public async Task<IActionResult> Lista(string? nombre, string? marca, string? categoria, bool? activo)
    {
        var productos = new List<ProductoRepuesto>();

        await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync("Get_Productos",
            ("@Nombre", nombre),
            ("@Marca", marca),
            ("@Categoria", categoria),
            ("@Activo", activo)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                productos.Add(new ProductoRepuesto
                {
                    IdProducto = reader.GetInt32(0),
                    PrNombre = reader.GetNullableString(1)!,
                    PrMarca = reader.GetNullableString(2),
                    PrCategoria = reader.GetNullableString(3),
                    PrCosto = reader.GetNullableDouble(4),
                    PrPrecioVenta = reader.GetNullableDouble(5),
                    PrStock = reader.GetNullableInt32(6) ?? 0
                });
            }
        }

        // Get_Productos no devuelve prActivo: se obtiene en una sola consulta
        var ids = productos.Select(p => p.IdProducto).ToList();
        var estados = await _bdTallerContext.ProductoRepuestos
            .Where(p => ids.Contains(p.IdProducto))
            .ToDictionaryAsync(p => p.IdProducto, p => p.PrActivo);

        foreach (var producto in productos)
            producto.PrActivo = estados.GetValueOrDefault(producto.IdProducto);

        ViewBag.FiltroNombre = nombre;
        ViewBag.FiltroMarca = marca;
        ViewBag.FiltroCategoria = categoria;
        ViewBag.FiltroActivo = activo;

        return View(productos);
    }

    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Crear()
    {
        await CargarMarcasYCategoriasAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Crear(string prNombre, string? prMarca, string? prCategoria, double? prCosto, double? prPrecioVenta)
    {
        try
        {
            await _bdTallerContext.Database.ExecuteSqlRawAsync(
                "EXEC Ins_Producto @prNombre, @prMarca, @prCategoria, @prCosto, @prPrecioVenta",
                new SqlParameter("@prNombre", prNombre),
                new SqlParameter("@prMarca", (object?)prMarca ?? DBNull.Value),
                new SqlParameter("@prCategoria", (object?)prCategoria ?? DBNull.Value),
                new SqlParameter("@prCosto", (object?)prCosto ?? DBNull.Value),
                new SqlParameter("@prPrecioVenta", (object?)prPrecioVenta ?? DBNull.Value));

            TempData["SuccessMessage"] = "Producto creado correctamente.";
            return RedirectToAction(nameof(Lista));
        }
        catch (SqlException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View();
        }
    }

    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Editar(int id)
    {
        var producto = await _bdTallerContext.ProductoRepuestos.FirstOrDefaultAsync(p => p.IdProducto == id);
        if (producto == null)
            return NotFound();

        await CargarMarcasYCategoriasAsync();
        return View(producto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Gestion)]
    public async Task<IActionResult> Editar(int id, string prNombre, string? prMarca, string? prCategoria, double? prCosto, double? prPrecioVenta, int? prStock)
    {
        try
        {
            await _bdTallerContext.Database.ExecuteSqlRawAsync(
                "EXEC Upd_Producto @idProducto, @prNombre, @prMarca, @prCategoria, @prCosto, @prPrecioVenta, @prStock",
                new SqlParameter("@idProducto", id),
                new SqlParameter("@prNombre", prNombre),
                new SqlParameter("@prMarca", (object?)prMarca ?? DBNull.Value),
                new SqlParameter("@prCategoria", (object?)prCategoria ?? DBNull.Value),
                new SqlParameter("@prCosto", (object?)prCosto ?? DBNull.Value),
                new SqlParameter("@prPrecioVenta", (object?)prPrecioVenta ?? DBNull.Value),
                new SqlParameter("@prStock", (object?)prStock ?? DBNull.Value));

            TempData["SuccessMessage"] = "Producto actualizado correctamente.";
            return RedirectToAction(nameof(Lista));
        }
        catch (SqlException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(new ProductoRepuesto
            {
                IdProducto = id,
                PrNombre = prNombre,
                PrMarca = prMarca,
                PrCategoria = prCategoria,
                PrCosto = prCosto,
                PrPrecioVenta = prPrecioVenta,
                PrStock = prStock ?? 0
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.AdminSupervisor)]
    public async Task<IActionResult> Toggle(int id)
    {
        await _bdTallerContext.Database.ExecuteSqlRawAsync(
            "EXEC Mod_EstadoProd @idProducto",
            new SqlParameter("@idProducto", id));

        return RedirectToAction(nameof(Lista));
    }

    #endregion

    #region Stock y devolución de compras

    [HttpGet]
    [Authorize(Roles = RolesSistema.AdminSupervisor)]
    public async Task<IActionResult> IncrementarStock(int id)
    {
        var producto = await _bdTallerContext.ProductoRepuestos.FirstOrDefaultAsync(p => p.IdProducto == id);
        if (producto == null)
            return NotFound();

        var proveedores = new List<dynamic>();
        await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync(
            "Get_ProveedoresPorProducto", ("@idProducto", id)))
        await using (var reader = await cmd.ExecuteReaderAsync())
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

        const string sqlUltimasCompras = """
            SELECT TOP 5
                   cp.idCompra,
                   cp.compFecha,
                   cpro.cproCantidad,
                   cpro.idProveedor,
                   pr.pNombre   AS ProveedorNombre,
                   cp.cprActivo AS CprActivo
              FROM Compra_Proveedores cp
              JOIN Compra_Producto cpro ON cpro.idCompra = cp.idCompra
              LEFT JOIN Proveedor pr    ON pr.idProveedor = cpro.idProveedor
             WHERE cpro.idProducto = @idProducto
             ORDER BY cp.compFecha DESC
            """;

        var ultimasCompras = new List<dynamic>();
        await using (var cmd = await _bdTallerContext.Database.CreateSqlCommandAsync(sqlUltimasCompras, ("@idProducto", id)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                ultimasCompras.Add(new
                {
                    IdCompra = reader.GetInt32(0),
                    CompFecha = reader.GetDateTime(1).ToString("g"),
                    CproCantidad = reader.GetInt32(2),
                    IdProveedor = reader.GetNullableInt32(3),
                    ProveedorNombre = reader.GetNullableString(4),
                    CprActivo = reader.GetBoolean(5)
                });
            }
        }

        ViewBag.Proveedores = proveedores;
        ViewBag.UltimasCompras = ultimasCompras;

        return View(producto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.AdminSupervisor)]
    public async Task<IActionResult> IncrementarStock(int id, int? IdProveedor, int cantidad)
    {
        await _bdTallerContext.Database.ExecuteSqlRawAsync(
            "EXEC Inc_StockProducto @idProducto, @cantidad, @idProveedor",
            new SqlParameter("@idProducto", id),
            new SqlParameter("@cantidad", cantidad),
            new SqlParameter("@idProveedor", (object?)IdProveedor ?? DBNull.Value));

        return RedirectToAction(nameof(Lista));
    }

    [HttpGet]
    [Authorize(Roles = RolesSistema.AdminSupervisor)]
    public async Task<IActionResult> Listar()
    {
        var compras = await _bdTallerContext.CompraProveedores
            .Where(c => c.CprActivo)
            .ToListAsync();

        return View(compras);
    }

    [HttpGet]
    [Authorize(Roles = RolesSistema.Administrador)]
    public async Task<IActionResult> Eliminar(int id)
    {
        var compra = await _bdTallerContext.CompraProveedores
            .FirstOrDefaultAsync(c => c.IdCompra == id && c.CprActivo);
        if (compra == null)
            return NotFound();

        return View(compra);
    }

    [HttpPost, ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RolesSistema.Administrador)]
    public async Task<IActionResult> EliminarConfirmado(int id)
    {
        // 1) Verificar que la compra exista y esté activa
        var compra = await _bdTallerContext.CompraProveedores.FirstOrDefaultAsync(c => c.IdCompra == id);
        if (compra == null)
        {
            TempData["ErrorMessage"] = $"La compra {id} no existe.";
            return RedirectToAction(nameof(Lista));
        }
        if (!compra.CprActivo)
        {
            TempData["ErrorMessage"] = $"La compra {id} ya fue devuelta.";
            return RedirectToAction(nameof(Lista));
        }

        // 2) Detalles de la compra: producto y cantidad
        var detalles = await _bdTallerContext.CompraProductos
            .Where(cp => cp.IdCompra == id && cp.CproActivo)
            .Select(cp => new { cp.IdProducto, cp.CproCantidad })
            .ToListAsync();

        // 3) Verificar que haya stock suficiente para restar cada detalle
        var insuficientes = new List<int>();
        foreach (var detalle in detalles)
        {
            var producto = await _bdTallerContext.ProductoRepuestos
                .FirstOrDefaultAsync(p => p.IdProducto == detalle.IdProducto);

            if (producto == null || producto.PrStock < detalle.CproCantidad)
                insuficientes.Add(detalle.IdProducto);
        }

        if (insuficientes.Count > 0)
        {
            TempData["ErrorMessage"] = "No es posible devolver la compra porque no hay suficiente stock para los productos: "
                + string.Join(", ", insuficientes);
            return RedirectToAction(nameof(IncrementarStock), new { id = detalles.First().IdProducto });
        }

        // 4) Todo correcto: devolver la compra
        try
        {
            await _bdTallerContext.Database.ExecuteSqlRawAsync(
                "EXEC Devolver_CompraProveedor @idCompra",
                new SqlParameter("@idCompra", id));

            TempData["SuccessMessage"] = $"Compra {id} devuelta correctamente.";
        }
        catch (SqlException ex)
        {
            TempData["ErrorMessage"] = $"Error al devolver la compra: {ex.Message}";
            return RedirectToAction(nameof(Lista));
        }

        // 5) Volver al stock del primer producto afectado
        return RedirectToAction(nameof(IncrementarStock), new { id = detalles.First().IdProducto });
    }

    #endregion

    #region Proveedores e historial

    [HttpGet]
    [Authorize(Roles = RolesSistema.Administrador)]
    public async Task<IActionResult> ProveedoresProducto(int idProducto)
    {
        var proveedores = await _bdTallerContext.ProveedorProductos
            .Where(pp => pp.IdProducto == idProducto && pp.Activo && pp.IdProveedorNavigation.PActivo)
            .Include(pp => pp.IdProveedorNavigation)
            .AsNoTracking()
            .ToListAsync();

        return View(proveedores);
    }

    [HttpGet]
    [Authorize(Roles = RolesSistema.Administrador)]
    public async Task<IActionResult> PorProducto(int idProducto)
    {
        var proveedores = new List<Proveedor>();

        await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync(
            "Get_ProveedoresPorProducto", ("@idProducto", idProducto)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                proveedores.Add(new Proveedor
                {
                    IdProveedor = reader.GetInt32(0),
                    PNombre = reader.GetString(1)
                });
            }
        }

        return Json(proveedores);
    }

    [HttpGet]
    [Authorize(Roles = RolesSistema.AdminSupervisor)]
    public async Task<IActionResult> HistorialProducto(int idProducto)
    {
        var historial = new DataTable();

        await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync(
            "Get_HistorialProducto", ("@idProducto", idProducto)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            historial.Load(reader);
        }

        ViewBag.IdProducto = idProducto;
        return View(historial);
    }

    #endregion

    private async Task CargarMarcasYCategoriasAsync()
    {
        ViewBag.Marcas = await _bdTallerContext.ProductoRepuestos
            .Where(p => !string.IsNullOrEmpty(p.PrMarca))
            .Select(p => p.PrMarca)
            .Distinct()
            .ToListAsync();

        ViewBag.Categorias = await _bdTallerContext.ProductoRepuestos
            .Where(p => !string.IsNullOrEmpty(p.PrCategoria))
            .Select(p => p.PrCategoria)
            .Distinct()
            .ToListAsync();
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
