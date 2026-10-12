using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers;

public class LoginController : Controller
{
    private const string MensajeTokenInvalido = "Token inválido o expirado.";

    private static readonly ConcurrentDictionary<string, (string Usuario, DateTime Expiracion, bool Usado)> RecoveryTokens = new();

    private readonly BdTallerContext _bd;

    public LoginController(BdTallerContext bd) => _bd = bd;

    private static byte[] HashPassword(string password) =>
        SHA256.HashData(Encoding.UTF8.GetBytes(password));

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction(nameof(MenuController.Menu), "Menu");

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;

        var empleado = await _bd.Empleados
            .Include(e => e.IdPersonaNavigation)
            .FirstOrDefaultAsync(e => e.EmpUsuario == username && e.EmpActivo);

        if (empleado?.EmpContrasena == null || !HashPassword(password).SequenceEqual(empleado.EmpContrasena))
        {
            ModelState.AddModelError(string.Empty, "Usuario o contraseña inválidos.");
            return View();
        }

        var rol = await _bd.Roles.FirstOrDefaultAsync(r => r.IdRol == empleado.IdRol);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, empleado.IdEmpleado.ToString()),
            new(ClaimTypes.Name, empleado.EmpUsuario),
            new(ClaimTypes.GivenName, empleado.IdPersonaNavigation?.PNombre ?? empleado.EmpUsuario),
            new(ClaimTypes.Surname, empleado.IdPersonaNavigation?.PApellido ?? string.Empty),
            new(ClaimTypes.Role, rol?.RTipo ?? string.Empty)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var properties = new AuthenticationProperties
        {
            AllowRefresh = true,
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), properties);

        return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? Redirect(returnUrl)
            : RedirectToAction(nameof(MenuController.Menu), "Menu");
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
        try
        {
            await _bd.Database.ExecuteSqlRawAsync(
                "EXEC Ins_Empleado @pNombre, @pApellido, @pTelefono, @pCedula, @empUsuario, @idRol, @contrasena_texto",
                new SqlParameter("@pNombre", pNombre ?? string.Empty),
                new SqlParameter("@pApellido", pApellido ?? string.Empty),
                new SqlParameter("@pTelefono", pTelefono ?? (object)DBNull.Value),
                new SqlParameter("@pCedula", pCedula ?? string.Empty),
                new SqlParameter("@empUsuario", empUsuario ?? string.Empty),
                new SqlParameter("@idRol", idRol),
                new SqlParameter("@contrasena_texto", contrasena_texto ?? string.Empty));

            return RedirectToAction(nameof(Login));
        }
        catch (SqlException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Roles = await _bd.Roles.ToListAsync();
            return View();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Salir() => Logout();

    [HttpGet]
    public IActionResult AccessDenied() => View();

    [AllowAnonymous]
    [HttpGet]
    public IActionResult SolicitarRecuperacion() => View();

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> SolicitarRecuperacion(string empUsuario)
    {
        if (string.IsNullOrEmpty(empUsuario))
        {
            ModelState.AddModelError(string.Empty, "Debe ingresar un usuario.");
            return View();
        }

        bool existe = await _bd.Empleados.AnyAsync(e => e.EmpUsuario == empUsuario);
        if (!existe)
        {
            ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
            return View();
        }

        var token = Guid.NewGuid().ToString();
        RecoveryTokens[token] = (empUsuario, DateTime.UtcNow.AddHours(1), false);

        var urlRecuperacion = Url.Action(nameof(RestablecerPassword), "Login", new { token }, Request.Scheme);
        ViewBag.Mensaje = $"Link para restablecer contraseña: <a href='{urlRecuperacion}' target='_blank'>{urlRecuperacion}</a>";

        return View();
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult RestablecerPassword(string token)
    {
        if (!TokenValido(token, out _))
            return BadRequest(MensajeTokenInvalido);

        return View(new RestablecerPasswordViewModel { Token = token });
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> RestablecerPassword(RestablecerPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (!TokenValido(model.Token, out var data))
            return BadRequest(MensajeTokenInvalido);

        var empleado = await _bd.Empleados.FirstOrDefaultAsync(e => e.EmpUsuario == data.Usuario);
        if (empleado == null)
        {
            ModelState.AddModelError(string.Empty, "Empleado no encontrado.");
            return View(model);
        }

        empleado.EmpContrasena = HashPassword(model.NuevaContrasena);
        RecoveryTokens[model.Token] = (data.Usuario, data.Expiracion, true);

        await _bd.SaveChangesAsync();
        TempData["Mensaje"] = "Contraseña actualizada correctamente.";
        return RedirectToAction(nameof(Login));
    }

    private static bool TokenValido(string? token, out (string Usuario, DateTime Expiracion, bool Usado) data)
    {
        data = default;
        return !string.IsNullOrEmpty(token)
            && RecoveryTokens.TryGetValue(token, out data)
            && !data.Usado
            && data.Expiracion >= DateTime.UtcNow;
    }

    public class RestablecerPasswordViewModel
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string NuevaContrasena { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NuevaContrasena), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContrasena { get; set; } = string.Empty;
    }
}
