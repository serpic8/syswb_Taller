using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using syswb_Taller.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers;

[Authorize(Roles = RolesSistema.Administrador)]
public class EmpleadoController : Controller
{
    private readonly BdTallerContext _bdTallerContext;

    public EmpleadoController(BdTallerContext bdTallerContext)
    {
        _bdTallerContext = bdTallerContext;
    }

    public class EmpleadoDto
    {
        public int IdEmpleado { get; set; }
        public string PNombre { get; set; } = string.Empty;
        public string PApellido { get; set; } = string.Empty;
        public string PTelefono { get; set; } = string.Empty;
        public string RTipo { get; set; } = string.Empty;
        public bool EmpActivo { get; set; }
    }

    // GET: Lista filtrada de empleados
    [HttpGet]
    public async Task<IActionResult> Lista(string? nombre, int? rol, bool? activo)
    {
        var empleados = new List<EmpleadoDto>();

        await using (var cmd = await _bdTallerContext.Database.CreateStoredProcedureAsync(
            "Get_Trabajadores", ("@Nombre", nombre), ("@Rol", rol), ("@Activo", activo)))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                empleados.Add(new EmpleadoDto
                {
                    IdEmpleado = reader.GetInt32(reader.GetOrdinal("idEmpleado")),
                    PNombre = reader.GetString(reader.GetOrdinal("pNombre")),
                    PApellido = reader.GetString(reader.GetOrdinal("pApellido")),
                    PTelefono = reader.GetString(reader.GetOrdinal("pTelefono")),
                    RTipo = reader.GetString(reader.GetOrdinal("Rol")),
                    EmpActivo = reader.GetBoolean(reader.GetOrdinal("Activo"))
                });
            }
        }

        ViewBag.NombreFiltro = nombre;
        ViewBag.RolFiltro = rol;
        ViewBag.ActivoFiltro = activo;

        return View(empleados);
    }

    // GET: Formulario de nuevo empleado
    [HttpGet]
    public async Task<IActionResult> Nuevo()
    {
        await CargarRolesAsync();
        return View();
    }

    // POST: Crear nuevo empleado
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Nuevo(string pNombre, string pApellido, string pTelefono, string pCedula, string empUsuario, int idRol, string contrasena_texto)
    {
        try
        {
            await _bdTallerContext.Database.ExecuteSqlRawAsync(
                "EXEC Ins_Empleado @pNombre, @pApellido, @pTelefono, @pCedula, @empUsuario, @idRol, @contrasena_texto",
                new SqlParameter("@pNombre", pNombre),
                new SqlParameter("@pApellido", pApellido),
                new SqlParameter("@pTelefono", pTelefono),
                new SqlParameter("@pCedula", pCedula),
                new SqlParameter("@empUsuario", empUsuario),
                new SqlParameter("@idRol", idRol),
                new SqlParameter("@contrasena_texto", contrasena_texto));

            TempData["SuccessMessage"] = "Empleado registrado correctamente.";
            return RedirectToAction(nameof(Lista));
        }
        catch (SqlException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View();
        }
    }

    // GET: Formulario de edición de empleado
    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var empleado = await BuscarEmpleadoAsync(id);
        if (empleado == null)
            return NotFound();

        await CargarRolesAsync();
        return View(empleado);
    }

    // POST: Actualizar empleado
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int idEmpleado, string pNombre, string pApellido, string pTelefono, string pCedula, string empUsuario, int idRol, string? contrasena_texto)
    {
        try
        {
            await _bdTallerContext.Database.ExecuteSqlRawAsync(
                "EXEC Upd_Empleado @idEmpleado, @pNombre, @pApellido, @pTelefono, @pCedula, @empUsuario, @idRol, @contrasena_texto",
                new SqlParameter("@idEmpleado", idEmpleado),
                new SqlParameter("@pNombre", pNombre),
                new SqlParameter("@pApellido", pApellido),
                new SqlParameter("@pTelefono", pTelefono),
                new SqlParameter("@pCedula", pCedula),
                new SqlParameter("@empUsuario", empUsuario),
                new SqlParameter("@idRol", idRol),
                new SqlParameter("@contrasena_texto", (object?)contrasena_texto ?? DBNull.Value));

            TempData["SuccessMessage"] = "Empleado actualizado correctamente.";
            return RedirectToAction(nameof(Lista));
        }
        catch (SqlException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(await BuscarEmpleadoAsync(idEmpleado));
        }
    }

    // POST: Alternar estado activo/inactivo
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleEstado(int idEmpleado)
    {
        try
        {
            await _bdTallerContext.Database.ExecuteSqlRawAsync(
                "EXEC mod_EstadoEmp @idEmpleado",
                new SqlParameter("@idEmpleado", idEmpleado));
        }
        catch (SqlException ex)
        {
            TempData["ErrorEmpleado"] = ex.Message;
        }

        return RedirectToAction(nameof(Lista));
    }

    private Task<Empleado?> BuscarEmpleadoAsync(int idEmpleado) =>
        _bdTallerContext.Empleados
            .Include(e => e.IdPersonaNavigation)
            .FirstOrDefaultAsync(e => e.IdEmpleado == idEmpleado);

    private async Task CargarRolesAsync()
    {
        ViewBag.Roles = await _bdTallerContext.Roles
            .Select(r => new Roles { IdRol = r.IdRol, RTipo = r.RTipo })
            .ToListAsync();
    }
}
