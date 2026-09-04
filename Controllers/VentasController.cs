using SCInvLibreria.Data;
using SCInvLibreria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace SCInvLibreria.Controllers;

public class VentasController : Controller
{
    private readonly LibreriaContext _context;
    public VentasController(LibreriaContext context) => _context = context;

    public async Task<IActionResult> Index(DateTime? desde, DateTime? hasta)
    {
        var query = _context.Ventas.Include(v => v.Libro).AsQueryable();
        if (desde.HasValue) query = query.Where(v => v.Fecha >= desde.Value.Date);
        if (hasta.HasValue) query = query.Where(v => v.Fecha < hasta.Value.Date.AddDays(1));
        ViewBag.Desde = desde?.ToString("yyyy-MM-dd");
        ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");
        return View(await query.OrderByDescending(v => v.Fecha).ThenByDescending(v => v.Id).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();
        var venta = await _context.Ventas.Include(v => v.Libro).FirstOrDefaultAsync(v => v.Id == id);
        return venta is null ? NotFound() : View(venta);
    }

    public async Task<IActionResult> Create()
    {
        await CargarLibros();
        return View(new Venta { Fecha = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Venta venta)
    {
        var libro = await _context.Libros.FindAsync(venta.LibroId);
        if (libro is null) ModelState.AddModelError(nameof(venta.LibroId), "Seleccione un libro válido.");
        else if (venta.Cantidad > libro.Stock) ModelState.AddModelError(nameof(venta.Cantidad), $"Stock insuficiente. Disponible: {libro.Stock}.");

        if (!ModelState.IsValid)
        {
            await CargarLibros(venta.LibroId);
            return View(venta);
        }

        using var tx = await _context.Database.BeginTransactionAsync();
        libro!.Stock -= venta.Cantidad;
        if (venta.PrecioUnitario <= 0) venta.PrecioUnitario = libro.Precio;
        _context.Ventas.Add(venta);
        await _context.SaveChangesAsync();
        await tx.CommitAsync();
        TempData["Ok"] = "Venta registrada y stock actualizado.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var venta = await _context.Ventas.FindAsync(id);
        if (venta is null) return NotFound();
        await CargarLibros(venta.LibroId);
        return View(venta);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Venta venta)
    {
        if (id != venta.Id) return NotFound();
        var original = await _context.Ventas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
        if (original is null) return NotFound();

        var libroOriginal = await _context.Libros.FindAsync(original.LibroId);
        var libroNuevo = original.LibroId == venta.LibroId ? libroOriginal : await _context.Libros.FindAsync(venta.LibroId);
        if (libroOriginal is null || libroNuevo is null)
            ModelState.AddModelError(nameof(venta.LibroId), "Seleccione un libro válido.");
        else
        {
            var disponible = libroNuevo.Stock + (original.LibroId == venta.LibroId ? original.Cantidad : 0);
            if (venta.Cantidad > disponible)
                ModelState.AddModelError(nameof(venta.Cantidad), $"Stock insuficiente. Disponible para esta edición: {disponible}.");
        }

        if (!ModelState.IsValid)
        {
            await CargarLibros(venta.LibroId);
            return View(venta);
        }

        using var tx = await _context.Database.BeginTransactionAsync();
        libroOriginal!.Stock += original.Cantidad;
        libroNuevo!.Stock -= venta.Cantidad;
        _context.Ventas.Update(venta);
        await _context.SaveChangesAsync();
        await tx.CommitAsync();
        TempData["Ok"] = "Venta actualizada y stock recalculado.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var venta = await _context.Ventas.Include(v => v.Libro).FirstOrDefaultAsync(v => v.Id == id);
        return venta is null ? NotFound() : View(venta);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var venta = await _context.Ventas.Include(v => v.Libro).FirstOrDefaultAsync(v => v.Id == id);
        if (venta is null) return NotFound();
        using var tx = await _context.Database.BeginTransactionAsync();
        venta.Libro!.Stock += venta.Cantidad;
        _context.Ventas.Remove(venta);
        await _context.SaveChangesAsync();
        await tx.CommitAsync();
        TempData["Ok"] = "Venta eliminada y stock devuelto.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerLibro(int id)
    {
        var libro = await _context.Libros.Where(l => l.Id == id).Select(l => new { l.Precio, l.Stock }).FirstOrDefaultAsync();
        return libro is null ? NotFound() : Json(libro);
    }

    private async Task CargarLibros(int? seleccionado = null)
    {
        var libros = await _context.Libros.OrderBy(l => l.Titulo).ToListAsync();
        ViewBag.LibroId = new SelectList(libros, "Id", "Titulo", seleccionado);
    }
}
