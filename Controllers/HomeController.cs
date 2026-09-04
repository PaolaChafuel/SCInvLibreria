using System.Diagnostics;
using SCInvLibreria.Data;
using SCInvLibreria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SCInvLibreria.Controllers;

public class HomeController : Controller
{
    private readonly LibreriaContext _context;
    public HomeController(LibreriaContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalLibros = await _context.Libros.CountAsync();
        ViewBag.TotalProveedores = await _context.Proveedores.CountAsync();
        ViewBag.TotalVentas = await _context.Ventas.SumAsync(v => (int?)v.Cantidad) ?? 0;
        ViewBag.StockTotal = await _context.Libros.SumAsync(l => (int?)l.Stock) ?? 0;
        ViewBag.BajoStock = await _context.Libros.Where(l => l.Stock <= 5).OrderBy(l => l.Stock).Take(5).ToListAsync();
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
