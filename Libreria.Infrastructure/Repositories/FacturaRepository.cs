using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Libreria.Core.Entities;
using Libreria.Core.Interfaces;
using Libreria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Libreria.Infrastructure.Repositories
{
    public class FacturaRepository : BaseRepository<Factura>, IFacturaRepository
    {
        private readonly ApplicationDbContext _context;

        public FacturaRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Factura>> GetAllAsync()
        {
            return await _context.Facturas.ToListAsync();
        }

        public async Task<Factura?> GetByIdAsync(int id)
        {
            return await _context.Facturas.FindAsync(id);
        }

        // === Implementaciones nuevas, coherentes con el servicio ===
        public async Task UpdateAsync(Factura factura)
        {
            _context.Facturas.Update(factura);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Factura factura)
        {
            _context.Facturas.Remove(factura);
            await _context.SaveChangesAsync();
        }
        // === fin nuevas implementaciones ===

        public async Task<IEnumerable<Factura>> GetFacturasPorClienteAsync(int clienteId)
        {
            return await _context.Facturas
                .Where(f => f.ClienteId == clienteId)
                .Include(f => f.DetalleFacturas)
                .ToListAsync();
        }

        public async Task<IEnumerable<Factura>> GetAllWithDetailsAsync()
        {
            return await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.DetalleFacturas)
                .ThenInclude(df => df.Libro)
                .ToListAsync();
        }

        public async Task<Factura?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.DetalleFacturas)
                .ThenInclude(df => df.Libro)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
