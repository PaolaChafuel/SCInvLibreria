using SCInvLibreria.Data;
using SCInvLibreria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace SCInvLibreria.Controllers;

public class LibrosController : Controller
{
    private readonly LibreriaContext _context;
    public LibrosController(LibreriaContext context) => _context = context;

    public async Task<IActionResult> Index(string? buscar)
    {
        var query = _context.Libros.Include(l => l.Proveedor).AsQueryable();
        if (!string.IsNullOrWhiteSpace(buscar))
            query = query.Where(l => l.Titulo.Contains(buscar) || l.Autor.Contains(buscar) || l.Isbn.Contains(buscar));
        ViewBag.Buscar = buscar;
        return View(await query.OrderBy(l => l.Titulo).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var libro = await _context.Libros.Include(l => l.Proveedor).FirstOrDefaultAsync(l => l.Id == id);
        return libro is null ? NotFound() : View(libro);
    }

    public async Task<IActionResult> Create()
    {
        await CargarProveedores();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Libro libro)
    {
        if (await _context.Libros.AnyAsync(l => l.Isbn == libro.Isbn))
            ModelState.AddModelError(nameof(libro.Isbn), "Ya existe un libro con este ISBN.");
        if (!ModelState.IsValid)
        {
            await CargarProveedores(libro.ProveedorId);
            return View(libro);
        }
        _context.Add(libro);
        await _context.SaveChangesAsync();
        TempData["Ok"] = "Libro creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var libro = await _context.Libros.FindAsync(id);
        if (libro is null) return NotFound();
        await CargarProveedores(libro.ProveedorId);
        return View(libro);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Libro libro)
    {
        if (id != libro.Id) return NotFound();
        if (await _context.Libros.AnyAsync(l => l.Isbn == libro.Isbn && l.Id != libro.Id))
            ModelState.AddModelError(nameof(libro.Isbn), "Ya existe otro libro con este ISBN.");
        if (!ModelState.IsValid)
        {
            await CargarProveedores(libro.ProveedorId);
            return View(libro);
        }
        _context.Update(libro);
        await _context.SaveChangesAsync();
        TempData["Ok"] = "Libro actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var libro = await _context.Libros.Include(l => l.Proveedor).Include(l => l.Ventas).FirstOrDefaultAsync(l => l.Id == id);
        return libro is null ? NotFound() : View(libro);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var libro = await _context.Libros.Include(l => l.Ventas).FirstOrDefaultAsync(l => l.Id == id);
        if (libro is null) return NotFound();
        if (libro.Ventas.Any())
        {
            TempData["Error"] = "No se puede eliminar el libro porque tiene ventas registradas.";
            return RedirectToAction(nameof(Index));
        }
        _context.Libros.Remove(libro);
        await _context.SaveChangesAsync();
        TempData["Ok"] = "Libro eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarProveedores(int? seleccionado = null)
    {
        ViewBag.ProveedorId = new SelectList(await _context.Proveedores.OrderBy(p => p.Nombre).ToListAsync(), "Id", "Nombre", seleccionado);
    }
}
