using Libreria.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Libreria.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<Libro> Libros { get; }
        IBaseRepository<Cliente> Clientes { get; }
        IBaseRepository<Autor> Autores { get; }
        IBaseRepository<Factura> Facturas { get; }
        IBaseRepository<DetalleFactura> DetallesFactura { get; }


        void SaveChanges();
        Task SaveChangesAsync();
    }
}
