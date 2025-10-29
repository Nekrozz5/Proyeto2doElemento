
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libreria.Core.Entities;
using Libreria.Core.Interfaces;
using Libreria.Infrastructure.Data;
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

        public async Task<IEnumerable<Factura>> GetAllAsync()
        {
            return await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.DetalleFacturas)    
                    .ThenInclude(d => d.Libro)
                .ToListAsync();
        }


        public async Task<Factura?> GetByIdAsync(int id)
        {
            return await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.DetalleFacturas)
                    .ThenInclude(d => d.Libro)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task AddAsync(Factura factura)
        {
            _context.Facturas.Add(factura);
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

        public async Task<IEnumerable<Factura>> GetFacturasPorClienteAsync(int clienteId)
        {
            return await _context.Facturas
                .Include(f => f.DetalleFacturas)
                    .ThenInclude(d => d.Libro)
                .Where(f => f.ClienteId == clienteId)
                .ToListAsync();
        }

    }
}
