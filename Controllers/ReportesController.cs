using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SCInvLibreria.Data;
using SCInvLibreria.ViewModels;
using System.Text;

namespace SCInvLibreria.Controllers;

public class ReportesController : Controller
{
    private readonly LibreriaContext _context;

    public ReportesController(LibreriaContext context)
    {
        _context = context;
    }

    // =====================================================
    // REPORTE DE VENTAS POR RANGO DE FECHAS
    // =====================================================

    [HttpGet]
    public async Task<IActionResult> VentasMensuales(
        DateTime? fechaDesde,
        DateTime? fechaHasta)
    {
        DateTime desde = fechaDesde
            ?? new DateTime(
                DateTime.Today.Year,
                DateTime.Today.Month,
                1
            );

        DateTime hasta = fechaHasta
            ?? DateTime.Today;

        // Validar rango
        if (desde.Date > hasta.Date)
        {
            ModelState.AddModelError(
                "",
                "La fecha inicial no puede ser mayor que la fecha final."
            );

            desde = new DateTime(
                DateTime.Today.Year,
                DateTime.Today.Month,
                1
            );

            hasta = DateTime.Today;
        }

        // Para incluir todo el último día
        DateTime hastaExclusiva =
            hasta.Date.AddDays(1);

        var ventas = await _context.Ventas
            .AsNoTracking()
            .Include(v => v.Libro)
            .Where(v =>
                v.Fecha >= desde.Date &&
                v.Fecha < hastaExclusiva
            )
            .ToListAsync();

        var detalles = ventas
            .Where(v => v.Libro != null)
            .GroupBy(v => new
            {
                v.LibroId,
                v.Libro!.Titulo,
                v.Libro.Autor
            })
            .Select(g => new DetalleReporteVenta
            {
                LibroId = g.Key.LibroId,

                Titulo = g.Key.Titulo,

                Autor = g.Key.Autor,

                CantidadVendida =
                    g.Sum(x => x.Cantidad),

                TotalVendido =
                    g.Sum(x =>
                        x.Cantidad *
                        x.PrecioUnitario
                    )
            })
            .OrderByDescending(x =>
                x.CantidadVendida
            )
            .ToList();

        var modelo = new ReporteMensualViewModel
        {
            FechaDesde = desde,
            FechaHasta = hasta,
            Detalles = detalles
        };

        return View(
            "VentasMensuales",
            modelo
        );
    }

    // =====================================================
    // DESCARGAR REPORTE EN CSV
    // =====================================================

    [HttpGet]
    public async Task<IActionResult> DescargarVentas(
        DateTime fechaDesde,
        DateTime fechaHasta)
    {
        if (fechaDesde.Date > fechaHasta.Date)
        {
            return BadRequest(
                "La fecha inicial no puede ser mayor que la fecha final."
            );
        }

        DateTime hastaExclusiva =
            fechaHasta.Date.AddDays(1);

        var ventas = await _context.Ventas
            .AsNoTracking()
            .Include(v => v.Libro)
            .Where(v =>
                v.Fecha >= fechaDesde.Date &&
                v.Fecha < hastaExclusiva
            )
            .ToListAsync();

        var detalles = ventas
            .Where(v => v.Libro != null)
            .GroupBy(v => new
            {
                v.LibroId,
                v.Libro!.Titulo,
                v.Libro.Autor
            })
            .Select(g => new DetalleReporteVenta
            {
                LibroId = g.Key.LibroId,

                Titulo = g.Key.Titulo,

                Autor = g.Key.Autor,

                CantidadVendida =
                    g.Sum(x => x.Cantidad),

                TotalVendido =
                    g.Sum(x =>
                        x.Cantidad *
                        x.PrecioUnitario
                    )
            })
            .OrderByDescending(x =>
                x.CantidadVendida
            )
            .ToList();

        StringBuilder csv = new();

        csv.AppendLine(
            "Libro;Autor;Cantidad vendida;Total vendido"
        );

        foreach (var item in detalles)
        {
            string titulo =
                item.Titulo.Replace(";", ",");

            string autor =
                item.Autor.Replace(";", ",");

            csv.AppendLine(
                $"{titulo};" +
                $"{autor};" +
                $"{item.CantidadVendida};" +
                $"{item.TotalVendido:F2}"
            );
        }

        csv.AppendLine();

        csv.AppendLine(
            $"TOTAL;;" +
            $"{detalles.Sum(x => x.CantidadVendida)};" +
            $"{detalles.Sum(x => x.TotalVendido):F2}"
        );

        // BOM UTF-8 para tildes y ñ en Excel
        byte[] bom =
            Encoding.UTF8.GetPreamble();

        byte[] contenido =
            Encoding.UTF8.GetBytes(
                csv.ToString()
            );

        byte[] archivo =
            new byte[
                bom.Length +
                contenido.Length
            ];

        Buffer.BlockCopy(
            bom,
            0,
            archivo,
            0,
            bom.Length
        );

        Buffer.BlockCopy(
            contenido,
            0,
            archivo,
            bom.Length,
            contenido.Length
        );

        string nombreArchivo =
            $"ReporteVentas_" +
            $"{fechaDesde:yyyy-MM-dd}_" +
            $"al_" +
            $"{fechaHasta:yyyy-MM-dd}.csv";

        return File(
            archivo,
            "text/csv; charset=utf-8",
            nombreArchivo
        );
    }
}