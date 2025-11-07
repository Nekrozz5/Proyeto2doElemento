using System.Collections.Generic;
using System.Threading.Tasks;
using Libreria.Core.Entities;

namespace Libreria.Core.Interfaces
{
    public interface IFacturaRepository : IBaseRepository<Factura>
    {
        // Obtener una factura simple por Id (sin relaciones)
        Task<Factura?> GetByIdAsync(int id);

        // Obtener todas las facturas sin incluir relaciones
        Task<IEnumerable<Factura>> GetAllAsync();

        // === Firmas usadas por FacturaService ===
        Task UpdateAsync(Factura factura);
        Task DeleteAsync(Factura factura);

        // Consultas con relaciones
        Task<IEnumerable<Factura>> GetFacturasPorClienteAsync(int clienteId);
        Task<IEnumerable<Factura>> GetAllWithDetailsAsync();
        Task<Factura?> GetByIdWithDetailsAsync(int id);
    }
}
