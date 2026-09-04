using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCInvLibreria.Models;

public class Venta
{
    public int Id { get; set; }

    [Display(Name = "Libro")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un libro.")]
    public int LibroId { get; set; }

    public Libro? Libro { get; set; }

    [Range(1, 9999)]
    public int Cantidad { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Fecha de venta")]
    public DateTime Fecha { get; set; } = DateTime.Today;

    [Range(0.01, 999999.99,
     ErrorMessage = "El precio unitario debe estar entre 0.01 y 999999.99.")]
    public decimal PrecioUnitario { get; set; }

    [NotMapped]
    [Display(Name = "Total")]
    public decimal Total => Cantidad * PrecioUnitario;
}
