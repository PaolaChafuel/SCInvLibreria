namespace SCInvLibreria.ViewModels;

public class ReporteMensualViewModel
{
    public DateTime FechaDesde { get; set; }

    public DateTime FechaHasta { get; set; }

    public List<DetalleReporteVenta> Detalles { get; set; } = new();

    public int TotalLibrosVendidos =>
        Detalles.Sum(x => x.CantidadVendida);

    public decimal TotalVentas =>
        Detalles.Sum(x => x.TotalVendido);
}

public class DetalleReporteVenta
{
    public int LibroId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Autor { get; set; } = string.Empty;

    public int CantidadVendida { get; set; }

    public decimal TotalVendido { get; set; }
}