using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Libreria.Core.Entities;
using Libreria.Core.Interfaces;

namespace Libreria.Core.Services
{
    public class ClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            return await _clienteRepository.GetAllAsync();
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _clienteRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Cliente cliente)
        {
            // Evita correos duplicados
            var existentes = await _clienteRepository.GetAllAsync();
            if (existentes.Any(c => c.Email.ToLower() == cliente.Email.ToLower()))
                throw new Exception("Ya existe un cliente con ese correo electrónico.");

            await _clienteRepository.AddAsync(cliente);
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            await _clienteRepository.UpdateAsync(cliente);
        }

        public async Task DeleteAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                throw new Exception("El cliente no existe.");

            await _clienteRepository.DeleteAsync(cliente);
        }
    }
}
