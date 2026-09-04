using SCInvLibreria.Models;
using Microsoft.EntityFrameworkCore;

namespace SCInvLibreria.Data;

public class LibreriaContext : DbContext
{
    public LibreriaContext(DbContextOptions<LibreriaContext> options) : base(options) { }

    public DbSet<Libro> Libros => Set<Libro>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Venta> Ventas => Set<Venta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Proveedor>().HasIndex(x => x.Ruc).IsUnique();
        modelBuilder.Entity<Libro>().HasIndex(x => x.Isbn).IsUnique();

        modelBuilder.Entity<Libro>()
            .HasOne(x => x.Proveedor)
            .WithMany(x => x.Libros)
            .HasForeignKey(x => x.ProveedorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Venta>()
            .HasOne(x => x.Libro)
            .WithMany(x => x.Ventas)
            .HasForeignKey(x => x.LibroId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
