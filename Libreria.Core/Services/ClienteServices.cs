using Dapper;
using Libreria.Core.Entities;
using Libreria.Core.Exceptions;
using Libreria.Core.Interfaces;
using Libreria.Core.QueryFilters;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Libreria.Core.CustomEntities;
using System.Threading.Tasks;

namespace Libreria.Core.Services
{
    public class ClienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDapperContext _dapper;

        public ClienteService(IUnitOfWork unitOfWork, IDapperContext dapper)
        {
            _unitOfWork = unitOfWork;
            _dapper = dapper;
        }

        // ==========================================================
        // GET: Todos los clientes con resumen de facturas
        // ==========================================================
        public IEnumerable<object> GetAll()
        {
            var clientes = _unitOfWork.Clientes
                .Query()
                .Include(c => c.Facturas)
                .AsNoTracking()
                .Select(c => new
                {
                    c.Id,
                    c.Nombre,
                    c.Apellido,
                    c.Email,
                    c.FechaRegistro,
                    c.FechaActualizacion,
                    Facturas = c.Facturas.Select(f => new { f.Id })
                })
                .ToList();

            if (!clientes.Any())
                throw new NotFoundException("No se encontraron clientes registrados.");

            return clientes;
        }

        // ==========================================================
        // GET: Cliente por Id con resumen de facturas
        // ==========================================================
        public async Task<object?> GetByIdAsync(int id)
        {
            var cliente = await _unitOfWork.Clientes
                .Query()
                .Include(c => c.Facturas)
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.Nombre,
                    c.Apellido,
                    c.Email,
                    c.FechaRegistro,
                    c.FechaActualizacion,
                    Facturas = c.Facturas.Select(f => new { f.Id })
                })
                .FirstOrDefaultAsync();

            if (cliente == null)
                throw new NotFoundException($"No se encontró el cliente con ID {id}.");

            return cliente;
        }

        // ==========================================================
        // POST: Crear nuevo cliente
        // ==========================================================
        public async Task AddAsync(Cliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.Nombre))
                throw new DomainValidationException("El nombre del cliente es obligatorio.");

            await _unitOfWork.Clientes.Add(cliente);
            await _unitOfWork.SaveChangesAsync();
        }

        // ==========================================================
        // PUT: Actualizar cliente
        // ==========================================================
        public async Task UpdateAsync(Cliente cliente)
        {
            if (cliente.Id <= 0)
                throw new DomainValidationException("Debe especificar un ID válido para actualizar.");

            var existing = await _unitOfWork.Clientes.GetById(cliente.Id);
            if (existing == null)
                throw new NotFoundException($"No se puede actualizar: el cliente con ID {cliente.Id} no existe.");

            _unitOfWork.Clientes.Update(cliente);
            await _unitOfWork.SaveChangesAsync();
        }

        // ==========================================================
        // DELETE: Eliminar cliente
        // ==========================================================
        public async Task DeleteAsync(int id)
        {
            var cliente = await _unitOfWork.Clientes.GetById(id);
            if (cliente == null)
                throw new NotFoundException($"No se puede eliminar: el cliente con ID {id} no existe.");

            await _unitOfWork.Clientes.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        // ==========================================================
        // FILTRADO + PAGINACIÓN (Dapper)
        // ==========================================================
        public async Task<PagedList<Cliente>> GetFilteredAsync(ClienteQueryFilter filters)
        {
            var sql = @"SELECT 
                            c.Id, c.Nombre, c.Apellido, c.Email,
                            COUNT(f.Id) AS FacturasRealizadas
                        FROM Clientes c
                        LEFT JOIN Facturas f ON c.Id = f.ClienteId
                        WHERE 1=1 ";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(filters.Nombre))
            {
                sql += " AND LOWER(c.Nombre) LIKE CONCAT('%', LOWER(@Nombre), '%')";
                parameters.Add("@Nombre", filters.Nombre);
            }

            if (!string.IsNullOrWhiteSpace(filters.Apellido))
            {
                sql += " AND LOWER(c.Apellido) LIKE CONCAT('%', LOWER(@Apellido), '%')";
                parameters.Add("@Apellido", filters.Apellido);
            }

            var countSql = $"SELECT COUNT(*) FROM ({sql} GROUP BY c.Id) AS CountQuery";
            var totalCount = await _dapper.ExecuteScalarAsync<int>(countSql, parameters);

            var offset = (filters.PageNumber - 1) * filters.PageSize;
            sql += " GROUP BY c.Id ORDER BY c.Nombre LIMIT @PageSize OFFSET @Offset";
            parameters.Add("@PageSize", filters.PageSize);
            parameters.Add("@Offset", offset);

            var items = await _dapper.QueryAsync<Cliente>(sql, parameters);
            return new PagedList<Cliente>(
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
                            c.Id,
                            CONCAT(c.Nombre, ' ', c.Apellido) AS ClienteNombre,
                            COUNT(f.Id) AS FacturasRealizadas
                        FROM Clientes c
                        LEFT JOIN Facturas f ON c.Id = f.ClienteId
                        GROUP BY c.Id, c.Nombre, c.Apellido
                        ORDER BY c.Nombre;";

            return await _dapper.QueryAsync<dynamic>(sql);
        }
    }
}
