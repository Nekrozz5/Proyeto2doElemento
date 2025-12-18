using Dapper;
using Libreria.Core.Entities;
using Libreria.Core.Exceptions;
using Libreria.Core.Interfaces;
using Libreria.Core.QueryFilters;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Libreria.Core.CustomEntities;


namespace Libreria.Core.Services
{
    public class FacturaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDapperContext _dapper;

        public FacturaService(IUnitOfWork unitOfWork, IDapperContext dapper)
        {
            _unitOfWork = unitOfWork;
            _dapper = dapper;
        }

        // ======================================================
        // GET COMPLETO (EF Core)
        // ======================================================
        public async Task<IEnumerable<Factura>> GetAllAsync()
        {
            var facturas = await _unitOfWork.Facturas
                .Query()
                .Include(f => f.Cliente)
                .Include(f => f.DetalleFacturas)
                    .ThenInclude(df => df.Libro)
                .ToListAsync();

            if (!facturas.Any())
                throw new NotFoundException("No se encontraron facturas registradas.");

            return facturas;
        }

        // ======================================================
        // GET BY ID (EF)
        // ======================================================
        public async Task<Factura?> GetByIdAsync(int id)
        {
            var factura = await _unitOfWork.Facturas
                .Query()
                .Include(f => f.Cliente)
                .Include(f => f.DetalleFacturas)
                    .ThenInclude(df => df.Libro)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (factura == null)
                throw new NotFoundException($"No se encontró la factura con ID {id}.");

            return factura;
        }

        // ======================================================
        // POST: Crear factura
        // ======================================================
        public async Task AddAsync(Factura factura)
        {
            if (factura.ClienteId <= 0)
                throw new DomainValidationException("Debe especificar un cliente válido.");

            if (factura.DetalleFacturas == null || !factura.DetalleFacturas.Any())
                throw new DomainValidationException("La factura debe tener al menos un detalle.");

            // Validar cliente
            var cliente = await _unitOfWork.Clientes.GetById(factura.ClienteId);
            if (cliente == null)
                throw new NotFoundException("El cliente no existe.");

            decimal total = 0;

            // Validar stock y calcular total
            foreach (var detalle in factura.DetalleFacturas)
            {
                var libro = await _unitOfWork.Libros.GetById(detalle.LibroId);

                if (libro == null)
                    throw new NotFoundException($"Libro con ID {detalle.LibroId} no encontrado.");

                if (detalle.Cantidad <= 0)
                    throw new DomainValidationException("La cantidad debe ser mayor a cero.");

                if (libro.Stock < detalle.Cantidad)
                    throw new BusinessRuleException(
                        $"Stock insuficiente del libro '{libro.Titulo}'. Disponible: {libro.Stock}");

                detalle.PrecioUnitario = libro.Precio;
                detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;

                total += detalle.Subtotal;

                libro.Stock -= detalle.Cantidad;
                _unitOfWork.Libros.Update(libro);
            }

            factura.Fecha = DateTime.Now;
            factura.Total = total;

            await _unitOfWork.Facturas.Add(factura);
            await _unitOfWork.SaveChangesAsync();
        }

        // ======================================================
        // PUT: Actualizar factura
        // ======================================================
        public async Task UpdateAsync(Factura factura)
        {
            if (factura.Id <= 0)
                throw new DomainValidationException("Debe especificar un ID válido.");

            var existing = await _unitOfWork.Facturas.GetById(factura.Id);
            if (existing == null)
                throw new NotFoundException($"Factura con ID {factura.Id} no existe.");

            _unitOfWork.Facturas.Update(factura);
            await _unitOfWork.SaveChangesAsync();
        }

        // ======================================================
        // DELETE: Eliminar factura
        // ======================================================
        public async Task DeleteAsync(int id)
        {
            var factura = await _unitOfWork.Facturas.GetById(id);

            if (factura == null)
                throw new NotFoundException($"No se puede eliminar: factura ID {id} no existe.");

            await _unitOfWork.Facturas.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        // ======================================================
        // GET FILTRADO (Dapper)
        // ======================================================
        public async Task<PagedList<Factura>> GetFilteredAsync(FacturaQueryFilter filters)
        {
            var sql = @"SELECT 
                            f.Id, f.Fecha, f.Total,
                            f.ClienteId,
                            CONCAT(c.Nombre, ' ', c.Apellido) AS ClienteNombre
                        FROM Facturas f
                        INNER JOIN Clientes c ON f.ClienteId = c.Id
                        WHERE 1=1 ";

            var parameters = new DynamicParameters();

            if (filters.ClienteId.HasValue)
            {
                sql += " AND f.ClienteId = @ClienteId";
                parameters.Add("@ClienteId", filters.ClienteId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filters.ClienteNombreContains))
            {
                sql += " AND LOWER(CONCAT(c.Nombre, ' ', c.Apellido)) LIKE CONCAT('%', LOWER(@Nombre), '%')";
                parameters.Add("@Nombre", filters.ClienteNombreContains);
            }

            if (filters.FechaDesde.HasValue)
            {
                sql += " AND f.Fecha >= @FechaDesde";
                parameters.Add("@FechaDesde", filters.FechaDesde.Value);
            }

            if (filters.FechaHasta.HasValue)
            {
                sql += " AND f.Fecha <= @FechaHasta";
                parameters.Add("@FechaHasta", filters.FechaHasta.Value);
            }

            if (filters.MinTotal.HasValue)
            {
                sql += " AND f.Total >= @MinTotal";
                parameters.Add("@MinTotal", filters.MinTotal.Value);
            }

            if (filters.MaxTotal.HasValue)
            {
                sql += " AND f.Total <= @MaxTotal";
                parameters.Add("@MaxTotal", filters.MaxTotal.Value);
            }

            // Total de registros
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS CountQuery";
            var totalCount = await _dapper.ExecuteScalarAsync<int>(countSql, parameters);

            // Paginación
            var offset = (filters.PageNumber - 1) * filters.PageSize;
            sql += " ORDER BY f.Fecha DESC LIMIT @PageSize OFFSET @Offset";

            parameters.Add("@PageSize", filters.PageSize);
            parameters.Add("@Offset", offset);

            var items = await _dapper.QueryAsync<Factura>(sql, parameters);

            return new PagedList<Factura>(
                items.ToList(),
                totalCount,
                filters.PageNumber,
                filters.PageSize
            );
        }

        // ======================================================
        // RESUMEN (Dapper)
        // ======================================================
        public async Task<IEnumerable<dynamic>> GetResumenAsync()
        {
            var sql = @"SELECT 
                            f.Id,
                            f.Fecha,
                            f.Total,
                            CONCAT(c.Nombre, ' ', c.Apellido) AS ClienteNombre
                        FROM Facturas f
                        INNER JOIN Clientes c ON f.ClienteId = c.Id
                        ORDER BY f.Fecha DESC";

            return await _dapper.QueryAsync<dynamic>(sql);
        }




        public async Task<IEnumerable<dynamic>>
            GetFacturacionDiariaAsync(DateTime fecha)
        {
            var inicio = fecha.Date;
            var fin = fecha.Date.AddDays(1).AddTicks(-1);

            var sql = @"
        SELECT 
            DATE(f.Fecha) AS Fecha,
            DAYNAME(f.Fecha) AS Dia,
            SUM(df.Subtotal) AS MontoTotal,
            SUM(df.Cantidad) AS CantidadLibros
        FROM Facturas f
        INNER JOIN DetalleFactura df ON f.Id = df.FacturaId
        WHERE f.Fecha BETWEEN @inicio AND @fin
        GROUP BY DATE(f.Fecha);
                ";

            return await _dapper.QueryAsync<dynamic>(
                sql,
                new { inicio, fin }
            );
        }


    }
}
