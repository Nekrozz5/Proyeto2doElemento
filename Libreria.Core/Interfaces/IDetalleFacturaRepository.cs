using System.Collections.Generic;
using System.Threading.Tasks;
using Libreria.Core.Entities;

namespace Libreria.Core.Interfaces
{
    public interface IDetalleFacturaRepository : IBaseRepository<DetalleFactura>
    {
        Task<IEnumerable<DetalleFactura>> GetByFacturaIdAsync(int facturaId);
    }
}
