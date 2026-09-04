using SCInvLibreria.Data;
using SCInvLibreria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SCInvLibreria.Controllers;

public class ProveedoresController : Controller
{
    private readonly LibreriaContext _context;
    public ProveedoresController(LibreriaContext context) => _context = context;

    public async Task<IActionResult> Index(string? buscar)
    {
        var query = _context.Proveedores.AsQueryable();
        if (!string.IsNullOrWhiteSpace(buscar))
            query = query.Where(p => p.Nombre.Contains(buscar) || p.Ruc.Contains(buscar));
        ViewBag.Buscar = buscar;
        return View(await query.OrderBy(p => p.Nombre).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var proveedor = await _context.Proveedores.Include(p => p.Libros).FirstOrDefaultAsync(p => p.Id == id);
        return proveedor is null ? NotFound() : View(proveedor);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Proveedor proveedor)
    {
        if (await _context.Proveedores.AnyAsync(p => p.Ruc == proveedor.Ruc))
            ModelState.AddModelError(nameof(proveedor.Ruc), "Ya existe un proveedor con este RUC/identificación.");
        if (!ModelState.IsValid) return View(proveedor);
        _context.Add(proveedor);
        await _context.SaveChangesAsync();
        TempData["Ok"] = "Proveedor creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var proveedor = await _context.Proveedores.FindAsync(id);
        return proveedor is null ? NotFound() : View(proveedor);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Proveedor proveedor)
    {
        if (id != proveedor.Id) return NotFound();
        if (await _context.Proveedores.AnyAsync(p => p.Ruc == proveedor.Ruc && p.Id != proveedor.Id))
            ModelState.AddModelError(nameof(proveedor.Ruc), "Ya existe otro proveedor con este RUC/identificación.");
        if (!ModelState.IsValid) return View(proveedor);
        _context.Update(proveedor);
        await _context.SaveChangesAsync();
        TempData["Ok"] = "Proveedor actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var proveedor = await _context.Proveedores.Include(p => p.Libros).FirstOrDefaultAsync(p => p.Id == id);
        return proveedor is null ? NotFound() : View(proveedor);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var proveedor = await _context.Proveedores.Include(p => p.Libros).FirstOrDefaultAsync(p => p.Id == id);
        if (proveedor is null) return NotFound();
        if (proveedor.Libros.Any())
        {
            TempData["Error"] = "No se puede eliminar el proveedor porque tiene libros asociados.";
            return RedirectToAction(nameof(Index));
        }
        _context.Proveedores.Remove(proveedor);
        await _context.SaveChangesAsync();
        TempData["Ok"] = "Proveedor eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
