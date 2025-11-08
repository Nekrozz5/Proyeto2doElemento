using Dapper;
using Libreria.Core.Entities;
using Libreria.Core.Exceptions;
using Libreria.Core.Interfaces;
using Libreria.Core.QueryFilters;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<Cliente> GetAll()
        {
            var clientes = _unitOfWork.Clientes.GetAll();

            if (clientes == null || !clientes.Any())
                throw new NotFoundException("No se encontraron clientes registrados.");

            return clientes;
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            var cliente = await _unitOfWork.Clientes.GetById(id);

            if (cliente == null)
                throw new NotFoundException($"No se encontró el cliente con ID {id}.");

            return cliente;
        }

        public async Task AddAsync(Cliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.Nombre))
                throw new DomainValidationException("El nombre del cliente es obligatorio.");

            if (string.IsNullOrWhiteSpace(cliente.Apellido))
                throw new DomainValidationException("El apellido del cliente es obligatorio.");

            if (string.IsNullOrWhiteSpace(cliente.Email))
                throw new DomainValidationException("El correo electrónico del cliente es obligatorio.");

            await _unitOfWork.Clientes.Add(cliente);
            await _unitOfWork.SaveChangesAsync();
        }

        public void Update(Cliente cliente)
        {
            if (cliente.Id <= 0)
                throw new DomainValidationException("Debe especificar un ID válido para actualizar.");

            var existing = _unitOfWork.Clientes.GetById(cliente.Id).Result;
            if (existing == null)
                throw new NotFoundException($"No se puede actualizar: el cliente con ID {cliente.Id} no existe.");

            _unitOfWork.Clientes.Update(cliente);
            _unitOfWork.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            var cliente = await _unitOfWork.Clientes.GetById(id);
            if (cliente == null)
                throw new NotFoundException($"No se puede eliminar: el cliente con ID {id} no existe.");

            await _unitOfWork.Clientes.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<Cliente>> GetFilteredAsync(ClienteQueryFilter filters)
        {
            var query = _unitOfWork.Clientes.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filters.Nombre))
                query = query.Where(c => c.Nombre.Contains(filters.Nombre));

            if (!string.IsNullOrWhiteSpace(filters.Apellido))
                query = query.Where(c => c.Apellido.Contains(filters.Apellido));

            if (!string.IsNullOrWhiteSpace(filters.EmailContains))
                query = query.Where(c => c.Email.Contains(filters.EmailContains));

            if (filters.ConFacturas == true)
                query = query.Where(c => c.Facturas.Any());

            return await query.ToListAsync();
        }

        // ======================
        // DAPPER: Reporte liviano de clientes
        // ======================
        public async Task<IEnumerable<dynamic>> GetResumenAsync()
        {
            var sql = @"SELECT 
                            c.Id,
                            CONCAT(c.Nombre, ' ', c.Apellido) AS ClienteNombre,
                            COUNT(f.Id) AS FacturasRegistradas,
                            COALESCE(SUM(f.Total), 0) AS MontoTotal
                        FROM Clientes c
                        LEFT JOIN Facturas f ON f.ClienteId = c.Id
                        GROUP BY c.Id, c.Nombre, c.Apellido
                        ORDER BY ClienteNombre;";

            return await _dapper.QueryAsync<dynamic>(sql);
        }
    }
}
