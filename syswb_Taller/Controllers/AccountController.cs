using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace syswb_Taller.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            ViewBag.Message = "No tienes permisos suficientes para realizar esta acción.";
            return View();
        }
    }
}
