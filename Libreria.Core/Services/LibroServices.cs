using System.Collections.Generic;
using System.Threading.Tasks;
using Libreria.Core.Entities;
using Libreria.Core.Interfaces;

namespace Libreria.Core.Services
{
    public class LibroService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LibroService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Libro> GetAll()
        {
            return _unitOfWork.Libros.GetAll();
        }

        public async Task<Libro?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Libros.GetById(id);
        }

        public async Task AddAsync(Libro libro)
        {
            await _unitOfWork.Libros.AddAsync(libro);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Libro libro)
        {
            _unitOfWork.Libros.Update(libro);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.Libros.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
