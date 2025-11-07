using Libreria.Core.Entities;
using Libreria.Core.Interfaces;
using System.Collections.Generic;
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
            await _unitOfWork.Clientes.Add(cliente);
            await _unitOfWork.SaveChangesAsync();
        }

        public void Update(Cliente cliente)
        {
            _unitOfWork.Clientes.Update(cliente);
            _unitOfWork.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.Clientes.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
