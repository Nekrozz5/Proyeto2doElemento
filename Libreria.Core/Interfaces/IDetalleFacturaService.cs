using System.Collections.Generic;
using System.Threading.Tasks;
using Libreria.Core.Entities;

namespace Libreria.Core.Interfaces
{
    public interface IDetalleFacturaService
    {
        IEnumerable<DetalleFactura> GetAll();
        Task<DetalleFactura?> GetByIdAsync(int id);
        Task<IEnumerable<DetalleFactura>> GetByFacturaIdAsync(int facturaId);
        Task AddAsync(DetalleFactura detalle);
        Task UpdateAsync(DetalleFactura detalle);
        Task DeleteAsync(int id);
    }
}
