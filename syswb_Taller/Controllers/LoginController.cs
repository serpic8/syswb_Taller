using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using syswb_Taller.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace syswb_Taller.Controllers
{
    public class LoginController : Controller
    {
        private readonly BdTallerContext _bd;
        private static ConcurrentDictionary<string, (string Usuario, DateTime Expiracion, bool Usado)> recoveryTokens = new();

        public LoginController(BdTallerContext bd) => _bd = bd;

        // Método utilitario para hashear contraseña
        private byte[] HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

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
            var empleado = await _bd.Empleados
                .Include(e => e.IdPersonaNavigation)
                .FirstOrDefaultAsync(e => e.EmpUsuario == username && e.EmpActivo);

            if (empleado == null || !HashPassword(password).SequenceEqual(empleado.EmpContrasena))
            {
                ModelState.AddModelError("", "Usuario o contraseña inválidos.");
                return View();
            }

            var rol = await _bd.Roles.FirstOrDefaultAsync(r => r.IdRol == empleado.IdRol);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, empleado.IdEmpleado.ToString()),
                new Claim(ClaimTypes.Name, empleado.EmpUsuario),
                new Claim(ClaimTypes.GivenName, empleado.IdPersonaNavigation.PNombre),   
                new Claim(ClaimTypes.Surname,   empleado.IdPersonaNavigation.PApellido),
                new Claim(ClaimTypes.Role, rol?.RTipo ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var properties = new AuthenticationProperties { AllowRefresh = true, IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

            return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Menu", "Menu");
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            ViewBag.Roles = _bd.Roles.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(string pNombre, string pApellido, string pTelefono, string pCedula, string empUsuario, int idRol, string contrasena_texto)
        {
            var p1 = new SqlParameter("@pNombre", pNombre);
            var p2 = new SqlParameter("@pApellido", pApellido);
            var p3 = new SqlParameter("@pTelefono", pTelefono);
            var p4 = new SqlParameter("@pCedula", pCedula);
            var p5 = new SqlParameter("@empUsuario", empUsuario);
            var p6 = new SqlParameter("@idRol", idRol);
            var p7 = new SqlParameter("@contrasena_texto", contrasena_texto);

            await _bd.Database.ExecuteSqlRawAsync("EXEC Ins_Empleado @pNombre, @pApellido, @pTelefono, @pCedula, @empUsuario, @idRol, @contrasena_texto", p1, p2, p3, p4, p5, p6, p7);

            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult SolicitarRecuperacion() => View();

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SolicitarRecuperacion(string empUsuario)
        {
            if (string.IsNullOrEmpty(empUsuario))
            {
                ModelState.AddModelError("", "Debe ingresar un usuario.");
                return View();
            }

            var empleado = await _bd.Empleados.FirstOrDefaultAsync(e => e.EmpUsuario == empUsuario);
            if (empleado == null)
            {
                ModelState.AddModelError("", "Usuario no encontrado.");
                return View();
            }

            var token = Guid.NewGuid().ToString();
            recoveryTokens[token] = (empUsuario, DateTime.UtcNow.AddHours(1), false);

            var urlRecuperacion = Url.Action("RestablecerPassword", "Login", new { token = token }, Request.Scheme);
            ViewBag.Mensaje = $"Link para restablecer contraseña: <a href='{urlRecuperacion}' target='_blank'>{urlRecuperacion}</a>";

            return View(); // vuelve a mostrar la misma vista
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult RestablecerPassword(string token)
        {
            if (string.IsNullOrEmpty(token) || !recoveryTokens.ContainsKey(token)) return BadRequest("Token inválido o expirado.");

            var (usuario, expiracion, usado) = recoveryTokens[token];
            if (usado || expiracion < DateTime.UtcNow) return BadRequest("Token inválido o expirado.");

            return View(new RestablecerPasswordViewModel { Token = token });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RestablecerPassword(RestablecerPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (!recoveryTokens.TryGetValue(model.Token, out var data)) return BadRequest("Token inválido o expirado.");
            if (data.Usado || data.Expiracion < DateTime.UtcNow) return BadRequest("Token inválido o expirado.");

            var empleado = await _bd.Empleados.FirstOrDefaultAsync(e => e.EmpUsuario == data.Usuario);
            if (empleado == null)
            {
                ModelState.AddModelError("", "Empleado no encontrado.");
                return View(model);
            }

            empleado.EmpContrasena = HashPassword(model.NuevaContrasena);
            recoveryTokens[model.Token] = (data.Usuario, data.Expiracion, true);

            await _bd.SaveChangesAsync();
            TempData["Mensaje"] = "Contraseña actualizada correctamente.";
            return RedirectToAction("Login");
        }

        public class RestablecerPasswordViewModel
        {
            [Required]
            public string Token { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [MinLength(8)]
            public string NuevaContrasena { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("NuevaContrasena", ErrorMessage = "Las contraseñas no coinciden.")]
            public string ConfirmarContrasena { get; set; }
        }
    }
}
