using Libreria.Core.Entities;
using Libreria.Core.Interfaces;
using Libreria.Infrastructure.Data;

namespace Libreria.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IBaseRepository<Libro> _libros;
        private IBaseRepository<Autor> _autores;
        private IBaseRepository<Cliente> _clientes;
        private IFacturaRepository _facturas;
        private IDetalleFacturaRepository _detallesFactura;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IBaseRepository<Libro> Libros => _libros ??= new BaseRepository<Libro>(_context);
        public IBaseRepository<Autor> Autores => _autores ??= new BaseRepository<Autor>(_context);
        public IBaseRepository<Cliente> Clientes => _clientes ??= new BaseRepository<Cliente>(_context);
        public IFacturaRepository Facturas => _facturas ??= new FacturaRepository(_context);
        public IDetalleFacturaRepository DetallesFactura => _detallesFactura ??= new DetalleFacturaRepository(_context);

        public System.Threading.Tasks.Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
        public int SaveChanges() => _context.SaveChanges();
        public void Dispose() => _context.Dispose();
    }
}
