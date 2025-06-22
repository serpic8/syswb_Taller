using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using syswb_Taller.Models;
using Microsoft.EntityFrameworkCore;

namespace syswb_Taller.Controllers
{
    public class LoginController : Controller
    {
        private readonly BdTallerContext _bd;

        public LoginController(BdTallerContext bd) => _bd = bd;

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Menu", "Menu");
            
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            // 1) Consulta al empleado activo
            dynamic emp = null;
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT e.idEmpleado,
                    e.empUsuario,
                    dbo.DecryptPassword(e.empContrasena) AS Pass,
                    r.rTipo
                    FROM Empleado e
                    JOIN Roles    r ON r.idRol = e.idRol
                    WHERE e.empUsuario = @u
                    AND e.empActivo  = 1";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@u", username));

                using var rd = await cmd.ExecuteReaderAsync();
                if (await rd.ReadAsync())
                {
                    emp = new
                    {
                        Id = rd.GetInt32(0),
                        User = rd.GetString(1),
                        Pass = rd.IsDBNull(2) ? null : rd.GetString(2),
                        Rol = rd.GetString(3)
                    };
                }
            }

            if (emp == null || emp.Pass != password)
            {
                ModelState.AddModelError("", "Usuario o contraseña inválidos.");
                return View();
            }

            // 2) Crear lista de Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, emp.Id.ToString()),
                new Claim(ClaimTypes.Name,           emp.User),
                new Claim(ClaimTypes.Role,           emp.Rol)
            };

            // 3) Construir ClaimsIdentity
            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            // 4) Configurar AuthenticationProperties
            var properties = new AuthenticationProperties
            {
                AllowRefresh = true,                                           // permite renovar la cookie
                IsPersistent = true,                                           // persiste tras cerrar el navegador
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)               // tiempo de vida de la cookie
            };

            // 5) Firmar la cookie enviando el ClaimsPrincipal
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            // 6) Redirigir a la URL original o al menú
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Menu", "Menu");
        }




        // GET: /Account/Registrar
        [HttpGet]
        public IActionResult Registrar()
        {
            // Para poblar dropdown de Roles (si lo deseas):
            ViewBag.Roles = _bd.Roles.ToList();
            return View();
        }

        // POST: /Account/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(
            string pNombre,
            string pApellido,
            string pTelefono,
            string pCedula,
            string empUsuario,
            int idRol,
            string contrasena_texto
        )
        {
            // Llamar a tu SP Ins_Empleado
            using var conn = _bd.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "Ins_Empleado";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@pNombre", pNombre));
            cmd.Parameters.Add(new SqlParameter("@pApellido", pApellido));
            cmd.Parameters.Add(new SqlParameter("@pTelefono", pTelefono));
            cmd.Parameters.Add(new SqlParameter("@pCedula", pCedula));
            cmd.Parameters.Add(new SqlParameter("@empUsuario", empUsuario));
            cmd.Parameters.Add(new SqlParameter("@idRol", idRol));
            cmd.Parameters.Add(new SqlParameter("@contrasena_texto", contrasena_texto));
            await cmd.ExecuteNonQueryAsync();

            // Tras registrar, redirige al Login
            return RedirectToAction(nameof(Login));
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }


    }
}

