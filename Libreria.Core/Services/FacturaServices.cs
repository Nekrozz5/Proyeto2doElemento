using Libreria.Core.Entities;
using Libreria.Core.Exceptions;
using Libreria.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Libreria.Core.QueryFilters;

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
            var facturas = _unitOfWork.Facturas
                .GetAll()
                .AsQueryable()
                .Include(f => f.Cliente)
                .Include(f => f.DetalleFacturas)
                    .ThenInclude(df => df.Libro)
                .ToList();

            if (facturas == null || !facturas.Any())
                throw new NotFoundException("No se encontraron facturas registradas.");

            return facturas;
        }

        // ======================
        // GET: Factura por Id
        // ======================
        public async Task<Factura?> GetByIdAsync(int id)
        {
            var factura = await _unitOfWork.Facturas
                .GetAll()
                .AsQueryable()
                .Include(f => f.Cliente)
                .Include(f => f.DetalleFacturas)
                    .ThenInclude(df => df.Libro)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (factura == null)
                throw new NotFoundException($"No se encontró la factura con ID {id}.");

            return factura;
        }

        // ======================
        // POST: Crear nueva factura
        // ======================
        public async Task AddAsync(Factura factura)
        {
            if (factura.ClienteId <= 0)
                throw new DomainValidationException("Debe especificar un cliente válido para la factura.");

            if (factura.DetalleFacturas == null || !factura.DetalleFacturas.Any())
                throw new DomainValidationException("La factura debe tener al menos un detalle.");

            // Buscar cliente
            var cliente = await _unitOfWork.Clientes.GetById(factura.ClienteId);
            if (cliente == null)
                throw new NotFoundException("Cliente no encontrado.");

            decimal total = 0;

            foreach (var detalle in factura.DetalleFacturas)
            {
                var libro = await _unitOfWork.Libros.GetById(detalle.LibroId);
                if (libro == null)
                    throw new NotFoundException($"Libro con ID {detalle.LibroId} no encontrado.");

                if (detalle.Cantidad <= 0)
                    throw new DomainValidationException("La cantidad del detalle debe ser mayor a cero.");

                if (libro.Stock < detalle.Cantidad)
                    throw new BusinessRuleException($"El libro '{libro.Titulo}' no tiene stock suficiente. Disponible: {libro.Stock}.");

                detalle.PrecioUnitario = libro.Precio;
                detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;
                total += detalle.Subtotal;

                // Actualizar stock
                libro.Stock -= detalle.Cantidad;
                _unitOfWork.Libros.Update(libro);
            }

            factura.Fecha = DateTime.Now;
            factura.Total = total;

            if (factura.Total <= 0)
                throw new BusinessRuleException("El total de la factura debe ser mayor que cero.");

            await _unitOfWork.Facturas.Add(factura);
            await _unitOfWork.SaveChangesAsync();
        }

        // ======================
        // PUT: Actualizar factura
        // ======================
        public void Update(Factura factura)
        {
            if (factura.Id <= 0)
                throw new DomainValidationException("Debe especificar un ID válido para actualizar la factura.");

            _unitOfWork.Facturas.Update(factura);
            _unitOfWork.SaveChanges();
        }

        // ======================
        // DELETE
        // ======================
        public async Task DeleteAsync(int id)
        {
            var factura = await _unitOfWork.Facturas.GetById(id);
            if (factura == null)
                throw new NotFoundException($"No se puede eliminar: la factura con ID {id} no existe.");

            await _unitOfWork.Facturas.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        // filtros

        public async Task<IEnumerable<Factura>> GetFilteredAsync(FacturaQueryFilter filters)
        {
            // 🔹 Definimos explícitamente como IQueryable
            IQueryable<Factura> query = _unitOfWork.Facturas.Query();

            // 🔹 Luego agregamos los Include
            query = query
                .AsNoTracking()
                .Include(f => f.Cliente)
                .Include(f => f.DetalleFacturas);

            // 🔹 Aplicamos los filtros paso a paso

            if (filters.ClienteId.HasValue)
                query = query.Where(f => f.ClienteId == filters.ClienteId.Value);

            if (!string.IsNullOrWhiteSpace(filters.ClienteNombreContains))
                query = query.Where(f =>
                    (f.Cliente!.Nombre + " " + f.Cliente!.Apellido)
                    .Contains(filters.ClienteNombreContains));

            if (filters.FechaDesde.HasValue)
                query = query.Where(f => f.Fecha >= filters.FechaDesde.Value);

            if (filters.FechaHasta.HasValue)
                query = query.Where(f => f.Fecha <= filters.FechaHasta.Value);

            if (filters.MinTotal.HasValue)
                query = query.Where(f => f.Total >= filters.MinTotal.Value);

            if (filters.MaxTotal.HasValue)
                query = query.Where(f => f.Total <= filters.MaxTotal.Value);

            // 🔹 Devolvemos la lista resultante
            return await query.ToListAsync();
        }
    }
}
