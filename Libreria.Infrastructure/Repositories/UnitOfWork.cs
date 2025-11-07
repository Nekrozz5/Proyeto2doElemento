using Libreria.Core.Entities;
using Libreria.Core.Interfaces;
using Libreria.Infrastructure.Data;
using System.Threading.Tasks;

namespace Libreria.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IBaseRepository<Libro>? _libros;
        private IBaseRepository<Cliente>? _clientes;
        private IBaseRepository<Autor>? _autores;
        private IBaseRepository<Factura>? _facturas;
        private IBaseRepository<DetalleFactura>? _detallesFactura;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IBaseRepository<Libro> Libros => _libros ??= new BaseRepository<Libro>(_context);
        public IBaseRepository<Cliente> Clientes => _clientes ??= new BaseRepository<Cliente>(_context);
        public IBaseRepository<Autor> Autores => _autores ??= new BaseRepository<Autor>(_context);
        public IBaseRepository<Factura> Facturas => _facturas ??= new BaseRepository<Factura>(_context);
        public IBaseRepository<DetalleFactura> DetallesFactura => _detallesFactura ??= new BaseRepository<DetalleFactura>(_context);

        public void SaveChanges() => _context.SaveChanges();
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}
