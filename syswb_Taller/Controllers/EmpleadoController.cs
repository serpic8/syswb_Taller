using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EmpleadoController : Controller
    {
        private readonly BdTallerContext _bdTallerContext;

        public EmpleadoController(BdTallerContext bdTallerContext)
        {
            _bdTallerContext = bdTallerContext;
        }

        // GET: Lista filtrada de empleados
        public class EmpleadoDto
        {
            public int IdEmpleado { get; set; }
            public string PNombre { get; set; }
            public string PApellido { get; set; }
            public string PTelefono { get; set; }
            public string RTipo { get; set; }
            public bool EmpActivo { get; set; }
        }

        // GET: Lista filtrada de empleados usando DTO y FromSqlRaw
        [HttpGet]
        public async Task<IActionResult> Lista(string nombre, int? rol, bool? activo)
        {
            var empleados = new List<EmpleadoDto>();

            var connection = _bdTallerContext.Database.GetDbConnection();

            try
            {
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "Get_Trabajadores";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                var pNombre = command.CreateParameter();
                pNombre.ParameterName = "@Nombre";
                pNombre.Value = (object?)nombre ?? DBNull.Value;
                command.Parameters.Add(pNombre);

                var pRol = command.CreateParameter();
                pRol.ParameterName = "@Rol";
                pRol.Value = (object?)rol ?? DBNull.Value;
                command.Parameters.Add(pRol);

                var pActivo = command.CreateParameter();
                pActivo.ParameterName = "@Activo";
                pActivo.Value = (object?)activo ?? DBNull.Value;
                command.Parameters.Add(pActivo);

                using var reader = await command.ExecuteReaderAsync();

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
            finally
            {
                await connection.CloseAsync();
            }

            ViewBag.NombreFiltro = nombre;
            ViewBag.RolFiltro = rol;
            ViewBag.ActivoFiltro = activo;

            return View(empleados);
        }






        // GET: Formulario de nuevo empleado
        [HttpGet]
        public IActionResult Nuevo()
        {
            ViewBag.Roles = _bdTallerContext.Roles
                    .Select(r => new Roles { IdRol = r.IdRol, RTipo = r.RTipo })
                    .ToList();

            return View();
        }

        // POST: Crear nuevo empleado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Nuevo(string pNombre, string pApellido, string pTelefono, string pCedula, string empUsuario, int idRol, string contrasena_texto)
        {
            try
            {
                var p1 = new SqlParameter("@pNombre", pNombre);
                var p2 = new SqlParameter("@pApellido", pApellido);
                var p3 = new SqlParameter("@pTelefono", pTelefono);
                var p4 = new SqlParameter("@pCedula", pCedula);
                var p5 = new SqlParameter("@empUsuario", empUsuario);
                var p6 = new SqlParameter("@idRol", idRol);
                var p7 = new SqlParameter("@contrasena_texto", contrasena_texto);

                await _bdTallerContext.Database.ExecuteSqlRawAsync(
                    "EXEC Ins_Empleado @pNombre, @pApellido, @pTelefono, @pCedula, @empUsuario, @idRol, @contrasena_texto",
                    p1, p2, p3, p4, p5, p6, p7
                );

                TempData["SuccessMessage"] = "Empleado registrado correctamente.";
                return RedirectToAction("Lista");
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: Formulario de edición de empleado
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var empleado = await _bdTallerContext.Empleados
                .Include(e => e.IdPersonaNavigation)
                .FirstOrDefaultAsync(e => e.IdEmpleado == id);

            ViewBag.Roles = _bdTallerContext.Roles
                    .Select(r => new Roles { IdRol = r.IdRol, RTipo = r.RTipo })
                    .ToList();

            if (empleado == null) return NotFound();

            return View(empleado);
        }

        // POST: Actualizar empleado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int idEmpleado, string pNombre, string pApellido, string pTelefono, string pCedula, string empUsuario, int idRol, string? contrasena_texto)
        {
            try
            {
                var p1 = new SqlParameter("@idEmpleado", idEmpleado);
                var p2 = new SqlParameter("@pNombre", pNombre);
                var p3 = new SqlParameter("@pApellido", pApellido);
                var p4 = new SqlParameter("@pTelefono", pTelefono);
                var p5 = new SqlParameter("@pCedula", pCedula);
                var p6 = new SqlParameter("@empUsuario", empUsuario);
                var p7 = new SqlParameter("@idRol", idRol);
                var p8 = new SqlParameter("@contrasena_texto", (object?)contrasena_texto ?? DBNull.Value);

                await _bdTallerContext.Database.ExecuteSqlRawAsync(
                    "EXEC Upd_Empleado @idEmpleado, @pNombre, @pApellido, @pTelefono, @pCedula, @empUsuario, @idRol, @contrasena_texto",
                    p1, p2, p3, p4, p5, p6, p7, p8
                );

                TempData["SuccessMessage"] = "Empleado actualizado correctamente.";
                return RedirectToAction("Lista");
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError("", ex.Message);

                var empleado = await _bdTallerContext.Empleados
                    .Include(e => e.IdPersonaNavigation)
                    .FirstOrDefaultAsync(e => e.IdEmpleado == idEmpleado);

                return View(empleado);
            }
        }

        // POST: Alternar estado activo/inactivo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEstado(int idEmpleado)
        {
            var p = new SqlParameter("@idEmpleado", idEmpleado);
            try
            {
                await _bdTallerContext.Database.ExecuteSqlRawAsync(
                    "EXEC mod_EstadoEmp @idEmpleado", p);
                return RedirectToAction("Lista");
            }
            catch (SqlException ex)
            {
                TempData["ErrorEmpleado"] = ex.Message;
                return RedirectToAction("Lista");
            }
        }
    }
}
