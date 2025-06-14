using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly BdTallerContext _bdTallerContext;


        public EmpleadoController(BdTallerContext bdTallerContext)
        {
            _bdTallerContext = bdTallerContext;
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<Empleado> lista = await _bdTallerContext.Empleados.ToListAsync();
            return View(lista);
        }

        [HttpGet]
        public IActionResult Nuevo()
        {
            return View();
        }

    }
}
