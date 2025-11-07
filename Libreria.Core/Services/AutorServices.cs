using Libreria.Core.Entities;
using Libreria.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Libreria.Core.Services
{
    public class AutorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AutorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Autor> GetAll()
        {
            return _unitOfWork.Autores.GetAll();
        }

        public async Task<Autor?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Autores.GetById(id);
        }

        public async Task AddAsync(Autor autor)
        {
            await _unitOfWork.Autores.Add(autor);
            await _unitOfWork.SaveChangesAsync();
        }

        public void Update(Autor autor)
        {
            _unitOfWork.Autores.Update(autor);
            _unitOfWork.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.Autores.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
