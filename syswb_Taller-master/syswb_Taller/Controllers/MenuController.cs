using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using syswb_Taller.Data;

namespace syswb_Taller.Controllers;

public class MenuController : Controller
{
    // La vista lee User.Identity y DateTime.Now directamente.
    [HttpGet]
    public IActionResult Menu() => View();

    // Accesos directos a los que apuntan los botones del menú
    public IActionResult Orden() => RedirectToAction(nameof(OrdenController.Lista), "Orden");
    public IActionResult Presupuesto() => RedirectToAction(nameof(PresupuestoController.Lista), "Presupuesto");
    public IActionResult Packs() => RedirectToAction(nameof(PackController.Lista), "Pack");
    public IActionResult Productos() => RedirectToAction(nameof(ProductoController.Lista), "Producto");
    public IActionResult Clientes() => RedirectToAction(nameof(ClienteController.Lista), "Cliente");
    public IActionResult Vehiculos() => RedirectToAction(nameof(VehiculoController.Lista), "Vehiculo");
    public IActionResult Servicios() => RedirectToAction(nameof(ServicioController.Lista), "Servicio");
    public IActionResult Administracion() => RedirectToAction(nameof(MovimientoFinancieroController.Resumen), "MovimientoFinanciero");

    public IActionResult CrearOrden() => RedirectToAction(nameof(OrdenController.CrearClienteVehiculo), "Orden");
    public IActionResult CrearPresupuesto() => RedirectToAction(nameof(PresupuestoController.CrearClienteVehiculo), "Presupuesto");

    [Authorize(Roles = RolesSistema.Cajero)]
    public IActionResult CajaDiaria() => RedirectToAction(nameof(CajaDiariaController.Lista), "CajaDiaria");

    [HttpGet]
    public IActionResult AbrirCajaDiaria() => RedirectToAction("Abrir", "Caja");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(LoginController.Login), "Login");
    }
}
