using System.Collections.Generic;
using System.Threading.Tasks;
using Libreria.Core.Entities;
using Libreria.Core.Interfaces;

namespace Libreria.Core.Services
{
    public class ClienteService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClienteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Cliente> GetAll()
        {
            return _unitOfWork.Clientes.GetAll();
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Clientes.GetById(id);
        }

        public async Task AddAsync(Cliente cliente)
        {
            await _unitOfWork.Clientes.AddAsync(cliente);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            _unitOfWork.Clientes.Update(cliente);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.Clientes.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
