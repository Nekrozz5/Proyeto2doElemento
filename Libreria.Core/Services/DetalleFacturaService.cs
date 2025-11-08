using Libreria.Core.Entities;
using Libreria.Core.Interfaces;
using Libreria.Core.QueryFilters;
using Microsoft.EntityFrameworkCore;
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


        // filtros

        public async Task<IEnumerable<DetalleFactura>> GetFilteredAsync(DetalleFacturaQueryFilter filters)
        {
            // 🔹 Define primero como IQueryable
            IQueryable<DetalleFactura> query = _unitOfWork.DetallesFactura.Query();

            // 🔹 Luego aplica los Includes (ahora no da conflicto)
            query = query
                .AsNoTracking()
                .Include(d => d.Factura)
                .Include(d => d.Libro);

            // 🔹 Aplica los filtros
            if (filters.FacturaId.HasValue)
                query = query.Where(d => d.FacturaId == filters.FacturaId.Value);

            if (filters.LibroId.HasValue)
                query = query.Where(d => d.LibroId == filters.LibroId.Value);

            if (!string.IsNullOrWhiteSpace(filters.LibroTituloContains))
                query = query.Where(d => d.Libro != null && d.Libro.Titulo.Contains(filters.LibroTituloContains));

            if (filters.MinCantidad.HasValue)
                query = query.Where(d => d.Cantidad >= filters.MinCantidad.Value);

            if (filters.MaxCantidad.HasValue)
                query = query.Where(d => d.Cantidad <= filters.MaxCantidad.Value);

            if (filters.MinPrecioUnitario.HasValue)
                query = query.Where(d => d.PrecioUnitario >= filters.MinPrecioUnitario.Value);

            if (filters.MaxPrecioUnitario.HasValue)
                query = query.Where(d => d.PrecioUnitario <= filters.MaxPrecioUnitario.Value);

            // 🔹 Devuelve la lista final
            return await query.ToListAsync();
        }
    }
}
