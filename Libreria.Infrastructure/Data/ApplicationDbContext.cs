using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Libreria.Core.Entities;

namespace Libreria.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // =====================================================
        // 🔹 TABLAS DEL DOMINIO
        // =====================================================
        public virtual DbSet<Libro> Libros { get; set; }
        public virtual DbSet<Autor> Autores { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<Factura> Facturas { get; set; }
        public virtual DbSet<DetalleFactura> DetalleFacturas { get; set; }

        // 🔹 NUEVA TABLA SECURITY PARA JWT
        public virtual DbSet<Security> Security { get; set; }

        // =====================================================
        // 🔹 CONFIGURACIÓN (APLICA TODAS LAS CONFIGURACIONES)
        // =====================================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica TODAS las clases en /Configurations/
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
