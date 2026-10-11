using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using syswb_Taller.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers;

[Authorize(Roles = RolesSistema.Administrador)]
public class ProveedorController : Controller
{
    private readonly BdTallerContext _bdTallerContext;

    public ProveedorController(BdTallerContext bdTallerContext)
    {
        _bdTallerContext = bdTallerContext;
    }

    // GET: Lista de proveedores con filtros
    public async Task<IActionResult> Lista(string? nombre, bool? activo)
    {
        var proveedores = await _bdTallerContext.Proveedores
            .FromSqlRaw("EXEC Get_Proveedores @Nombre, @Activo",
                new SqlParameter("@Nombre", (object?)nombre ?? DBNull.Value),
                new SqlParameter("@Activo", (object?)activo ?? DBNull.Value))
            .ToListAsync();

        ViewBag.NombreFiltro = nombre;
        ViewBag.ActivoFiltro = activo;
        return View(proveedores);
    }

    // GET: Formulario de creación
    [HttpGet]
    public IActionResult Detalle() => View();

    // POST: Crear nuevo proveedor
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Detalle(string pNombre, string pTelefono)
    {
        try
        {
            await _bdTallerContext.Database.ExecuteSqlRawAsync(
                "EXEC Ins_Proveedor @pNombre, @pTelefono",
                new SqlParameter("@pNombre", pNombre),
                new SqlParameter("@pTelefono", pTelefono));

            TempData["SuccessMessage"] = "Proveedor agregado correctamente.";
            return RedirectToAction(nameof(Lista));
        }
        catch (SqlException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View();
        }
    }

    // GET: Formulario de edición
    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var proveedor = await _bdTallerContext.Proveedores.FirstOrDefaultAsync(p => p.IdProveedor == id);
        if (proveedor == null)
            return NotFound();

        await CargarDatosProveedorAsync(id);
        return View(proveedor);
    }

    // POST: Guardar cambios de edición
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, string pNombre, string pTelefono)
    {
        try
        {
            await _bdTallerContext.Database.ExecuteSqlRawAsync(
                "EXEC Upd_Proveedor @idProveedor, @pNombre, @pTelefono",
                new SqlParameter("@idProveedor", id),
                new SqlParameter("@pNombre", pNombre),
                new SqlParameter("@pTelefono", pTelefono));

            TempData["SuccessMessage"] = "Proveedor actualizado correctamente.";
            return RedirectToAction(nameof(Lista));
        }
        catch (SqlException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            var proveedor = await _bdTallerContext.Proveedores.FirstOrDefaultAsync(p => p.IdProveedor == id);
            await CargarDatosProveedorAsync(id);
            return View(proveedor);
        }
    }

    // GET: Productos asociados como JSON
    [HttpGet]
    public async Task<IActionResult> Productos(int idProveedor)
    {
        var lista = await _bdTallerContext.Set<ProveedorProducto>()
            .FromSqlRaw("EXEC Get_ProductosPorProveedor @idProveedor", new SqlParameter("@idProveedor", idProveedor))
            .ToListAsync();

        return Json(lista);
    }

    // POST: Agregar producto asociado
    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> AgregarProducto(int idProveedor, int idProducto, double precioRef) =>
        EjecutarYVolverAEditarAsync(idProveedor,
            "EXEC Ins_ProveedorProducto @idProveedor, @idProducto, @precioRef",
            new SqlParameter("@idProveedor", idProveedor),
            new SqlParameter("@idProducto", idProducto),
            new SqlParameter("@precioRef", precioRef));

    // POST: Actualizar precio/estado del producto asociado
    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> ActualizarProducto(int idProveedor, int idProducto, double? precioRef, bool? activo) =>
        EjecutarYVolverAEditarAsync(idProveedor,
            "EXEC Upd_ProveedorProducto @idProveedor, @idProducto, @precioRef, @activo",
            new SqlParameter("@idProveedor", idProveedor),
            new SqlParameter("@idProducto", idProducto),
            new SqlParameter("@precioRef", (object?)precioRef ?? DBNull.Value),
            new SqlParameter("@activo", (object?)activo ?? DBNull.Value));

    // POST: Alternar estado activo del producto asociado
    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> ToggleProducto(int idProveedor, int idProducto) =>
        EjecutarYVolverAEditarAsync(idProveedor,
            "EXEC Toggle_ProveedorProducto @idProveedor, @idProducto",
            new SqlParameter("@idProveedor", idProveedor),
            new SqlParameter("@idProducto", idProducto));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleProveedor(int idProveedor)
    {
        try
        {
            await _bdTallerContext.Database.ExecuteSqlRawAsync(
                "EXEC Mod_EstadoProv @idProveedor",
                new SqlParameter("@idProveedor", idProveedor));
        }
        catch (SqlException ex)
        {
            TempData["ErrorProveedor"] = ex.Message;
        }

        return RedirectToAction(nameof(Lista));
    }

    /// <summary>
    /// Ejecuta un comando sobre los productos del proveedor y vuelve a su pantalla de edición.
    /// Si la base de datos devuelve un error, se muestra en la vista.
    /// </summary>
    private async Task<IActionResult> EjecutarYVolverAEditarAsync(int idProveedor, string sql, params SqlParameter[] parameters)
    {
        try
        {
            await _bdTallerContext.Database.ExecuteSqlRawAsync(sql, parameters);
        }
        catch (SqlException ex)
        {
            TempData["ErrorProducto"] = ex.Message;
        }

        return RedirectToAction(nameof(Editar), new { id = idProveedor });
    }

    /// <summary>
    /// Carga en ViewBag los productos activos y los productos asociados al proveedor.
    /// </summary>
    private async Task CargarDatosProveedorAsync(int id)
    {
        ViewBag.TodosProductos = await _bdTallerContext.ProductoRepuestos
            .Where(p => p.PrActivo)
            .ToListAsync();

        var asociados = new List<ProveedorProducto>();
        await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync(
            "dbo.Get_ProductosPorProveedor", ("@idProveedor", id)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
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
        }

        ViewBag.ProductosAsociados = asociados;
    }
}
