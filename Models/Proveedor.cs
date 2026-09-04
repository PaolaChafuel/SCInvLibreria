using System.ComponentModel.DataAnnotations;

namespace SCInvLibreria.Models;

public class Proveedor
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [Required, StringLength(20)]
    [Display(Name = "RUC / Identificación")]
    public string Ruc { get; set; } = string.Empty;

    [StringLength(120), EmailAddress]
    public string? Email { get; set; }

    [StringLength(30)]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    [StringLength(180)]
    [Display(Name = "Dirección")]
    public string? Direccion { get; set; }

    public ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
