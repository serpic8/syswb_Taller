using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers
{
    public class PersonaController : Controller
    {
        private readonly BdTallerContext _bdTallerContext;
        public PersonaController(BdTallerContext bdTallerContext)
        {
            _bdTallerContext = bdTallerContext;
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<Persona> lista = await _bdTallerContext.Personas.ToListAsync();
            return View(lista);
        }


        [HttpGet] //Aca es para devolver una vista
        public IActionResult Nuevo()
        {
           return View();
        }


        //Metodo para agregar informacion, el post devuelve los valosres de la vista Nuevo, podria aca llamr a los metodos que ya tengo listos
        [HttpPost]
        public async Task<IActionResult> Nuevo(Persona persona)
        {
            await _bdTallerContext.Personas.AddAsync(persona);
            await _bdTallerContext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }

        [HttpGet] //Aca es para devolver una vista
        public async Task<IActionResult> Editar(int id)
        {
            Persona persona = await _bdTallerContext.Personas.FirstAsync(e => e.IdPersona == id);
            return View(persona);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Persona persona)
        {
            _bdTallerContext.Personas.Update(persona);
            await _bdTallerContext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }


        [HttpGet] //Aca es para devolver una vista
        public async Task<IActionResult> Eliminar(int id)
        {
            Persona persona = await _bdTallerContext.Personas.FirstAsync(e => e.IdPersona == id);
            _bdTallerContext.Personas.Remove(persona);
            await _bdTallerContext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
