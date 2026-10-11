using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using syswb_Taller.Models;

namespace syswb_Taller.Controllers;

public class PersonaController : Controller
{
    private readonly BdTallerContext _bdTallerContext;

    public PersonaController(BdTallerContext bdTallerContext)
    {
        _bdTallerContext = bdTallerContext;
    }

    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> Lista()
    {
        var lista = await _bdTallerContext.Personas.ToListAsync();
        return View(lista);
    }

    [HttpGet]
    public IActionResult Nuevo() => View();

    [HttpPost]
    public async Task<IActionResult> Nuevo(Persona persona)
    {
        await _bdTallerContext.Personas.AddAsync(persona);
        await _bdTallerContext.SaveChangesAsync();
        return RedirectToAction(nameof(Lista));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var persona = await _bdTallerContext.Personas.FirstAsync(p => p.IdPersona == id);
        return View(persona);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(Persona persona)
    {
        _bdTallerContext.Personas.Update(persona);
        await _bdTallerContext.SaveChangesAsync();
        return RedirectToAction(nameof(Lista));
    }

    [HttpGet]
    public async Task<IActionResult> Eliminar(int id)
    {
        var persona = await _bdTallerContext.Personas.FirstAsync(p => p.IdPersona == id);
        _bdTallerContext.Personas.Remove(persona);
        await _bdTallerContext.SaveChangesAsync();
        return RedirectToAction(nameof(Lista));
    }
}
