
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libreria.Core.Interfaces;
using Libreria.Infrastructure.Data;
using Libreria.Core.Entities;
using Microsoft.EntityFrameworkCore;


namespace Libreria.Infrastructure.Repositories
{
    public class FacturaRepository : IFacturaRepository
    {
        private readonly ApplicationDbContext _context;

        public FacturaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Factura>> GetAllAsync() =>
            await _context.Facturas.ToListAsync();
        public async Task<Factura?> GetByIdAsync(int id) =>
            await _context.Facturas.FindAsync(id);

        public async Task AddAsync(Factura factura)
            {
            await _context.Facturas.AddAsync(factura);
            await _context.SaveChangesAsync();
        }
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
        public async Task<IEnumerable<Factura>> GetFacturasPorClienteAsync(int clienteId) =>
            await _context.Facturas.Where(f => f.ClienteId == clienteId).ToListAsync();
    }
}
