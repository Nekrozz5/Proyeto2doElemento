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

        public ClienteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ======================
        // GET: Todos los clientes
        // ======================
        public IEnumerable<Cliente> GetAll()
        {
            var clientes = _unitOfWork.Clientes.GetAll();

            if (clientes == null || !clientes.Any())
                throw new NotFoundException("No se encontraron clientes registrados.");

            return clientes;
        }

        // ======================
        // GET: Cliente por Id
        // ======================
        public async Task<Cliente?> GetByIdAsync(int id)
        {
            var cliente = await _unitOfWork.Clientes.GetById(id);

            if (cliente == null)
                throw new NotFoundException($"No se encontró el cliente con ID {id}.");

            return cliente;
        }

        // ======================
        // POST: Crear nuevo cliente
        // ======================
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

        // ======================
        // PUT: Actualizar cliente
        // ======================
        public void Update(Cliente cliente)
        {
            if (cliente.Id <= 0)
                throw new DomainValidationException("Debe especificar un ID válido para actualizar.");

            if (string.IsNullOrWhiteSpace(cliente.Nombre))
                throw new DomainValidationException("El nombre del cliente es obligatorio.");

            var existing = _unitOfWork.Clientes.GetById(cliente.Id).Result;
            if (existing == null)
                throw new NotFoundException($"No se puede actualizar: el cliente con ID {cliente.Id} no existe.");

            _unitOfWork.Clientes.Update(cliente);
            _unitOfWork.SaveChanges();
        }

        // ======================
        // DELETE: Eliminar cliente
        // ======================
        public async Task DeleteAsync(int id)
        {
            var cliente = await _unitOfWork.Clientes.GetById(id);
            if (cliente == null)
                throw new NotFoundException($"No se puede eliminar: el cliente con ID {id} no existe.");

            await _unitOfWork.Clientes.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        //filtros

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
    }
}
