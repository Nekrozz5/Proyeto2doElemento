using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Libreria.Core.Entities;
using Libreria.Core.Interfaces;
using Libreria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Libreria.Infrastructure.Repositories
{
    public class DetalleFacturaRepository : BaseRepository<DetalleFactura>, IDetalleFacturaRepository
    {
        private readonly ApplicationDbContext _context;

        public DetalleFacturaRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DetalleFactura>> GetByFacturaIdAsync(int facturaId)
        {
            return await _context.DetalleFacturas
                .Where(d => d.FacturaId == facturaId)
                .Include(d => d.Libro)
                .ToListAsync();
        }
    }

}
