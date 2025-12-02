using Dapper;
using Libreria.Core.Entities;
using Libreria.Core.Interfaces;
using Libreria.Core.QueryFilters;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Libreria.Core.CustomEntities;

namespace Libreria.Core.Services
{
    public class DetalleFacturaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDapperContext _dapper;

        public DetalleFacturaService(IUnitOfWork unitOfWork, IDapperContext dapper)
        {
            _unitOfWork = unitOfWork;
            _dapper = dapper;
        }

        // ==========================================================
        // GET: Todos los detalles (EF)
        // ==========================================================
        public IEnumerable<DetalleFactura> GetAll()
        {
            return _unitOfWork.DetalleFacturas.GetAll();
        }

        // ==========================================================
        // GET: DetalleFactura por Id
        // ==========================================================
        public async Task<DetalleFactura?> GetByIdAsync(int id)
        {
            return await _unitOfWork.DetalleFacturas.GetById(id);
        }

        // ==========================================================
        // POST: Crear nuevo detalle
        // ==========================================================
        public async Task AddAsync(DetalleFactura detalle)
        {
            detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;

            await _unitOfWork.DetalleFacturas.Add(detalle);
            await _unitOfWork.SaveChangesAsync();
        }

        // ==========================================================
        // PUT: Actualizar detalle
        // ==========================================================
        public async Task UpdateAsync(DetalleFactura detalle)
        {
            var existing = await _unitOfWork.DetalleFacturas.GetById(detalle.Id);

            if (existing == null)
                throw new KeyNotFoundException($"No existe el DetalleFactura con ID {detalle.Id}.");

            detalle.Subtotal = detalle.Cantidad * detalle.PrecioUnitario;

            _unitOfWork.DetalleFacturas.Update(detalle);
            await _unitOfWork.SaveChangesAsync();
        }

        // ==========================================================
        // DELETE: Eliminar detalle
        // ==========================================================
        public async Task DeleteAsync(int id)
        {
            var detalle = await _unitOfWork.DetalleFacturas.GetById(id);

            if (detalle == null)
                throw new KeyNotFoundException($"No existe el DetalleFactura con ID {id}.");

            await _unitOfWork.DetalleFacturas.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        // ==========================================================
        // FILTROS + PAGINACIÓN (Dapper)
        // ==========================================================
        public async Task<PagedList<DetalleFactura>> GetFilteredAsync(DetalleFacturaQueryFilter filters)
        {
            var sql = @"SELECT 
                            d.Id, d.FacturaId, d.LibroId, d.Cantidad, 
                            d.PrecioUnitario, d.Subtotal,
                            l.Titulo AS LibroTitulo
                        FROM DetallesFactura d
                        INNER JOIN Libros l ON d.LibroId = l.Id
                        WHERE 1=1 ";

            var parameters = new DynamicParameters();

            if (filters.FacturaId.HasValue)
            {
                sql += " AND d.FacturaId = @FacturaId";
                parameters.Add("@FacturaId", filters.FacturaId);
            }

            if (filters.LibroId.HasValue)
            {
                sql += " AND d.LibroId = @LibroId";
                parameters.Add("@LibroId", filters.LibroId);
            }

            if (!string.IsNullOrWhiteSpace(filters.LibroTituloContains))
            {
                sql += " AND LOWER(l.Titulo) LIKE CONCAT('%', LOWER(@Titulo), '%')";
                parameters.Add("@Titulo", filters.LibroTituloContains);
            }

            // Total de registros
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS CountQuery";
            var totalCount = await _dapper.ExecuteScalarAsync<int>(countSql, parameters);

            // Paginación
            var offset = (filters.PageNumber - 1) * filters.PageSize;
            sql += " ORDER BY d.FacturaId LIMIT @PageSize OFFSET @Offset";

            parameters.Add("@PageSize", filters.PageSize);
            parameters.Add("@Offset", offset);

            var items = await _dapper.QueryAsync<DetalleFactura>(sql, parameters);

            return new PagedList<DetalleFactura>(
                items.ToList(),
                totalCount,
                filters.PageNumber,
                filters.PageSize
            );
        }

        // ==========================================================
        // RESUMEN (Dapper)
        // ==========================================================
        public async Task<IEnumerable<dynamic>> GetResumenAsync()
        {
            var sql = @"SELECT 
                            d.Id,
                            d.FacturaId,
                            l.Titulo AS LibroTitulo,
                            d.Cantidad,
                            d.PrecioUnitario,
                            d.Subtotal
                        FROM DetallesFactura d
                        INNER JOIN Libros l ON d.LibroId = l.Id
                        ORDER BY d.FacturaId;";

            return await _dapper.QueryAsync<dynamic>(sql);
        }
    }
}
