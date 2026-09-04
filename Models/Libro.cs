using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCInvLibreria.Models;

public class Libro
{
    public int Id { get; set; }

    [Required, StringLength(160)]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string Autor { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string Isbn { get; set; } = string.Empty;

    [Range(0, 999999)]
    public int Stock { get; set; }

    [Range(0.01, 999999.99,
        ErrorMessage = "El precio debe estar entre 0.01 y 999999.99.")]
    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "Precio de venta")]
    public decimal Precio { get; set; }

    [Display(Name = "Proveedor")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un proveedor.")]
    public int ProveedorId { get; set; }

    public Proveedor? Proveedor { get; set; }

    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}