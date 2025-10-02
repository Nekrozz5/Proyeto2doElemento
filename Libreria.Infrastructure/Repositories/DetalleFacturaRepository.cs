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
    public class DetalleFacturaRepository : IDetalleFacturaRepository

    {
      private readonly ApplicationDbContext _context;
        public DetalleFacturaRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<DetalleFactura>> GetAllAsync() =>
            await _context.DetalleFacturas.ToListAsync();
        public async Task<DetalleFactura?> GetByIdAsync(int id) =>
            await _context.DetalleFacturas.FindAsync(id);
        public async Task AddAsync(DetalleFactura detalleFactura)
        {
            await _context.DetalleFacturas.AddAsync(detalleFactura);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(DetalleFactura detalleFactura)
        {
            _context.DetalleFacturas.Update(detalleFactura);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(DetalleFactura detalleFactura)
        {
            _context.DetalleFacturas.Remove(detalleFactura);
            await _context.SaveChangesAsync();
        }

    }
}
