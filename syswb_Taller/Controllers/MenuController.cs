using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.EntityFrameworkCore;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers
{
    //[Authorize] es necesario para hacer lo de los roles
    public class MenuController : Controller
    {
        private readonly BdTallerContext _bdTallerContext;

        public MenuController(BdTallerContext bdTallerContext)
        {
            _bdTallerContext = bdTallerContext;
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Menu()
        {
            bool cajaAbierta = await _bdTallerContext.CajaDiaria
                .AsNoTracking()
                .AnyAsync(c => c.CjCierre == null);

            ViewBag.CajaAbierta = cajaAbierta;

            return View();
        }
        // Ejemplos de acciones a las que apuntan los botones:
        public IActionResult Orden() => RedirectToAction("Lista", "Orden");
        public IActionResult Presupuesto() => RedirectToAction("Lista", "Presupuesto");
        public IActionResult Packs() => RedirectToAction("Lista", "Pack");
        public IActionResult Productos() => RedirectToAction("Lista", "Producto");
        public IActionResult Clientes() => RedirectToAction("Lista", "Cliente");
        public IActionResult Vehiculos() => RedirectToAction("Lista", "Vehiculo");
        public IActionResult Servicios() => RedirectToAction("Lista", "Servicio");
        public IActionResult Administracion() => RedirectToAction("Resumen", "MovimientoFinanciero");

        public IActionResult CrearOrden() => RedirectToAction("CrearClienteVehiculo", "Orden");
        public IActionResult CrearPresupuesto() => RedirectToAction("CrearClienteVehiculo", "Presupuesto");
        [Authorize(Roles = "Cajero")]
        public IActionResult CajaDiaria() => RedirectToAction("Lista", "CajaDiaria");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Borra la cookie de autenticación
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Redirige al Login
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public IActionResult AbrirCajaDiaria() => RedirectToAction("Abrir", "Caja");
    }
}
