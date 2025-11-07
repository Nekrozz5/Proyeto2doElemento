using Libreria.Core.Entities;
using Libreria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Libreria.Core.Services
{
    public class FacturaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FacturaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ======================
        // GET: Todas las facturas con cliente y detalles
        // ======================
        public IEnumerable<Factura> GetAll()
        {
            return _unitOfWork.Facturas
                .GetAll()
                .AsQueryable()
                .Include(f => f.Cliente)
                .Include(f => f.DetalleFacturas)
                    .ThenInclude(df => df.Libro);
        }

        // ======================
        // GET: Factura por Id
        // ======================
        public async Task<Factura?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Facturas
                .GetAll()
                .AsQueryable()
                .Include(f => f.Cliente)
                .Include(f => f.DetalleFacturas)
                    .ThenInclude(df => df.Libro)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        // ======================
        // POST: Crear nueva factura
        // ======================
        public async Task AddAsync(Factura factura)
        {
            // Buscar cliente
            var cliente = await _unitOfWork.Clientes.GetById(factura.ClienteId);
            if (cliente == null)
                throw new Exception("Cliente no encontrado.");

            // Calcular total y actualizar stock
            decimal total = 0;
            foreach (var detalle in factura.DetalleFacturas)
            {
                var libro = await _unitOfWork.Libros.GetById(detalle.LibroId);
                if (libro == null)
                    throw new Exception($"Libro con ID {detalle.LibroId} no encontrado.");

                detalle.PrecioUnitario = libro.Precio;
                detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;
                total += detalle.Subtotal;

                // Actualizar stock
                libro.Stock -= detalle.Cantidad;
                _unitOfWork.Libros.Update(libro);
            }

            factura.Fecha = DateTime.Now;
            factura.Total = total;

            await _unitOfWork.Facturas.Add(factura);
            await _unitOfWork.SaveChangesAsync();
        }

        // ======================
        // PUT: Actualizar factura
        // ======================
        public void Update(Factura factura)
        {
            _unitOfWork.Facturas.Update(factura);
            _unitOfWork.SaveChanges();
        }

        // ======================
        // DELETE
        // ======================
        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.Facturas.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
