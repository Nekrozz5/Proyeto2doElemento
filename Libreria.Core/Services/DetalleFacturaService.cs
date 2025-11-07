using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Libreria.Core.Entities;
using Libreria.Core.Interfaces;

namespace Libreria.Core.Services
{
    public class DetalleFacturaService : IDetalleFacturaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DetalleFacturaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ✅ Obtener todos los detalles
        public IEnumerable<DetalleFactura> GetAll()
        {
            return _unitOfWork.DetallesFactura.GetAll();
        }

        // ✅ Obtener detalle por ID
        public async Task<DetalleFactura?> GetByIdAsync(int id)
        {
            return await _unitOfWork.DetallesFactura.GetById(id);
        }

        // ✅ Obtener detalles de una factura específica
        public async Task<IEnumerable<DetalleFactura>> GetByFacturaIdAsync(int facturaId)
        {
            return await _unitOfWork.DetallesFactura.GetByFacturaIdAsync(facturaId);
        }

        // ✅ Insertar un nuevo detalle
        public async Task AddAsync(DetalleFactura detalle)
        {
            // Cálculo automático del subtotal
            detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;

            await _unitOfWork.DetallesFactura.AddAsync(detalle);
            await _unitOfWork.SaveChangesAsync();
        }

        // ✅ Actualizar un detalle existente
        public async Task UpdateAsync(DetalleFactura detalle)
        {
            detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;

            _unitOfWork.DetallesFactura.Update(detalle);
            await _unitOfWork.SaveChangesAsync();
        }

        // ✅ Eliminar un detalle
        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.DetallesFactura.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
