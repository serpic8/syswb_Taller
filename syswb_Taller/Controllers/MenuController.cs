using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using syswb_Taller.Data;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        private readonly BdTallerContext _bdTallerContext;

        public MenuController(BdTallerContext bdTallerContext)
        {
            _bdTallerContext = bdTallerContext;
        }

        [HttpGet]
        public async Task<IActionResult> Menu()
        {
            bool cajaAbierta = await _bdTallerContext.CajaDiaria
                .AsNoTracking()
                .AnyAsync(c => c.CjCierre == null);

            ViewBag.CajaAbierta = cajaAbierta;
            return View();
        }

        public IActionResult Orden() => RedirectToAction("Lista", "Orden");
        public IActionResult Presupuesto() => RedirectToAction("Lista", "Presupuesto");
        public IActionResult Packs() => RedirectToAction("Lista", "Pack");
        public IActionResult Productos() => RedirectToAction("Lista", "Producto");
        public IActionResult Clientes() => RedirectToAction("Lista", "Cliente");
        public IActionResult Vehiculos() => RedirectToAction("Lista", "Vehiculo");
        public IActionResult Servicios() => RedirectToAction("Lista", "Servicio");

        [Authorize(Roles = RolesSistema.AdminSupervisor)]
        public IActionResult Administracion() => RedirectToAction("Resumen", "MovimientoFinanciero");

        [Authorize(Roles = RolesSistema.Gestion)]
        public IActionResult CajaDiaria() => RedirectToAction("Lista", "CajaDiaria");

        [Authorize(Roles = RolesSistema.Gestion)]
        public IActionResult AbrirCajaDiaria() => RedirectToAction("Lista", "CajaDiaria");

        [Authorize(Roles = RolesSistema.Administrador)]
        public IActionResult Empleados() => RedirectToAction("Lista", "Empleado");

        [Authorize(Roles = RolesSistema.Administrador)]
        public IActionResult Proveedores() => RedirectToAction("Lista", "Proveedor");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(LoginController.Login), "Login");
        }
    }
}
