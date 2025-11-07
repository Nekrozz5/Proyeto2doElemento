using Libreria.Core.Entities;

namespace Libreria.Core.Interfaces
{
    public interface IUnitOfWork : System.IDisposable
    {
        IBaseRepository<Libro> Libros { get; }
        IBaseRepository<Autor> Autores { get; }
        IBaseRepository<Cliente> Clientes { get; }
        IFacturaRepository Facturas { get; }
        IDetalleFacturaRepository DetallesFactura { get; }   // ← usa la interfaz específica

        System.Threading.Tasks.Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}
