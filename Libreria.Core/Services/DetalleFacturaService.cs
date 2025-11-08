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

        public async Task<PagedList<DetalleFactura>> GetFilteredAsync(DetalleFacturaQueryFilter filters)
        {
            var sql = @"SELECT 
                    d.Id, d.FacturaId, d.LibroId, d.Cantidad, d.PrecioUnitario, d.Subtotal,
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

            // Totales
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS CountQuery";
            var totalCount = await _dapper.ExecuteScalarAsync<int>(countSql, parameters);

            var offset = (filters.PageNumber - 1) * filters.PageSize;
            sql += " ORDER BY d.FacturaId LIMIT @PageSize OFFSET @Offset";
            parameters.Add("@PageSize", filters.PageSize);
            parameters.Add("@Offset", offset);

            var items = await _dapper.QueryAsync<DetalleFactura>(sql, parameters);
            return new PagedList<DetalleFactura>(items.ToList(), totalCount, filters.PageNumber, filters.PageSize);
        }

        // ======================
        // DAPPER: Reporte liviano de detalles
        // ======================
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
