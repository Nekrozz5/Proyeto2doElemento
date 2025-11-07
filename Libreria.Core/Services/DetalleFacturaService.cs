using Libreria.Core.Entities;
using Libreria.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Libreria.Core.Services
{
    public class DetalleFacturaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DetalleFacturaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<DetalleFactura> GetAll()
        {
            return _unitOfWork.DetallesFactura.GetAll();
        }

        public async Task<DetalleFactura?> GetByIdAsync(int id)
        {
            return await _unitOfWork.DetallesFactura.GetById(id);
        }

        public async Task AddAsync(DetalleFactura detalle)
        {
            detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;
            await _unitOfWork.DetallesFactura.Add(detalle);
            await _unitOfWork.SaveChangesAsync();
        }

        public void Update(DetalleFactura detalle)
        {
            _unitOfWork.DetallesFactura.Update(detalle);
            _unitOfWork.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.DetallesFactura.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
