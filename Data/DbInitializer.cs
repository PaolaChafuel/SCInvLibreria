using SCInvLibreria.Models;

namespace SCInvLibreria.Data;

public static class DbInitializer
{
    public static void Seed(LibreriaContext context)
    {
        if (context.Proveedores.Any()) return;

        var proveedores = new[]
        {
            new Proveedor { Nombre = "Editorial Andina", Ruc = "1790012345001", Email = "ventas@andina.test", Telefono = "022345678", Direccion = "Quito" },
            new Proveedor { Nombre = "Distribuidora Lectura", Ruc = "1790098765001", Email = "pedidos@lectura.test", Telefono = "023456789", Direccion = "Quito" }
        };
        context.Proveedores.AddRange(proveedores);
        context.SaveChanges();

        var libros = new[]
        {
            new Libro { Titulo = "Fundamentos de Programación", Autor = "Ana Torres", Isbn = "9780000000011", Stock = 25, Precio = 18.50m, ProveedorId = proveedores[0].Id },
            new Libro { Titulo = "Bases de Datos Aplicadas", Autor = "Luis Pérez", Isbn = "9780000000028", Stock = 18, Precio = 22.90m, ProveedorId = proveedores[1].Id },
            new Libro { Titulo = "Redes para Principiantes", Autor = "María López", Isbn = "9780000000035", Stock = 12, Precio = 20.00m, ProveedorId = proveedores[0].Id }
        };
        context.Libros.AddRange(libros);
        context.SaveChanges();
    }
}
