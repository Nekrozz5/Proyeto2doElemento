using Libreria.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Core.Interfaces
{
    public interface IDetalleFacturaRepository
    {
        Task<IEnumerable<DetalleFactura>> GetAllAsync();
        Task<DetalleFactura?> GetByIdAsync(int id);
        Task AddAsync(DetalleFactura detalle);
        Task UpdateAsync(DetalleFactura detalle);
        Task DeleteAsync(DetalleFactura detalle);
    }
}
