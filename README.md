# Sistema de Control de Inventario para Librería

Proyecto monolítico ASP.NET Core MVC para Visual Studio 2022.

## Funcionalidades
- CRUD de Libros.
- CRUD de Proveedores.
- CRUD de Ventas.
- Relación Libro -> Proveedor.
- Relación Venta -> Libro.
- Control automático de stock al crear, editar y eliminar ventas.
- Validación de stock insuficiente.
- Restricción para eliminar proveedores con libros asociados.
- Restricción para eliminar libros con ventas asociadas.
- Reporte de libros vendidos por mes, unidades y total monetario.
- Filtros de búsqueda y fecha.
- Interfaz Razor + Bootstrap + JavaScript.
- MySQL (XAMPP) con creación automática de la base de datos.

## Cómo ejecutar
1. Abrir `SCInvLibreria.sln` en Visual Studio 2022.
2. Esperar a que Visual Studio restaure los paquetes NuGet.
3. Asegurarse de tener instalado el SDK de .NET 8 / carga de trabajo "ASP.NET y desarrollo web".
4. Ejecutar con IIS Express o con el perfil del proyecto (F5 o Ctrl+F5).
5. En el primer inicio se crea automáticamente `libreriaSCI` y se agregan datos iniciales de ejemplo.

## Base de datos
La cadena está en `appsettings.json`:
`Data Source=libreriaSCI`

No hace falta crear la base manualmente ni ejecutar migraciones para esta entrega porque el proyecto usa `Database.EnsureCreated()`.


## MySQL con XAMPP
1. Inicie **Apache** y **MySQL** desde el Panel de Control de XAMPP.
2. La aplicación usa MySQL en `127.0.0.1:3306`, usuario `root`, sin contraseña.
3. La base configurada es `libreriaSCI`.
4. Al iniciar la aplicación, Entity Framework Core crea la base y sus tablas automáticamente si el usuario MySQL tiene permiso para crear bases de datos.
5. Si prefiere crearla manualmente desde phpMyAdmin, cree una base llamada `libreriaSCI` con cotejamiento `utf8mb4_general_ci` y luego ejecute el proyecto.

La cadena se encuentra en `appsettings.json` bajo `ConnectionStrings:ConexionMysql`.
