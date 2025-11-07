using Libreria.Core.Entities;
using Libreria.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
            return _unitOfWork.Libros.Query()
               .Include(l => l.Autor)
               .AsNoTracking()
               .ToList();
        }

        public async Task<Libro?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Libros.GetById(id);
        }

        public async Task AddAsync(Libro libro)
        {
            await _unitOfWork.Libros.Add(libro);
            await _unitOfWork.SaveChangesAsync();
        }

        public void Update(Libro libro)
        {
            _unitOfWork.Libros.Update(libro);
            _unitOfWork.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.Libros.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
