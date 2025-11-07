using Libreria.Core.Entities;
using Libreria.Core.Exceptions;
using Libreria.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
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

        // ======================
        // GET: Todos los autores
        // ======================
        public IEnumerable<Autor> GetAll()
        {
            var autores = _unitOfWork.Autores.GetAll();

            if (autores == null || !autores.Any())
                throw new NotFoundException("No se encontraron autores registrados.");

            return autores;
        }

        // ======================
        // GET: Autor por Id
        // ======================
        public async Task<Autor?> GetByIdAsync(int id)
        {
            var autor = await _unitOfWork.Autores.GetById(id);

            if (autor == null)
                throw new NotFoundException($"No se encontró el autor con ID {id}.");

            return autor;
        }

        // ======================
        // POST: Crear nuevo autor
        // ======================
        public async Task AddAsync(Autor autor)
        {
            if (string.IsNullOrWhiteSpace(autor.Nombre))
                throw new DomainValidationException("El nombre del autor es obligatorio.");

            await _unitOfWork.Autores.Add(autor);
            await _unitOfWork.SaveChangesAsync();
        }

        // ======================
        // PUT: Actualizar autor
        // ======================
        public void Update(Autor autor)
        {
            if (autor.Id <= 0)
                throw new DomainValidationException("Debe especificar un ID válido para actualizar.");

            if (string.IsNullOrWhiteSpace(autor.Nombre))
                throw new DomainValidationException("El nombre del autor es obligatorio.");

            var existing = _unitOfWork.Autores.GetById(autor.Id).Result;
            if (existing == null)
                throw new NotFoundException($"No se puede actualizar: el autor con ID {autor.Id} no existe.");

            _unitOfWork.Autores.Update(autor);
            _unitOfWork.SaveChanges();
        }

        // ======================
        // DELETE: Eliminar autor
        // ======================
        public async Task DeleteAsync(int id)
        {
            var autor = await _unitOfWork.Autores.GetById(id);
            if (autor == null)
                throw new NotFoundException($"No se puede eliminar: el autor con ID {id} no existe.");

            await _unitOfWork.Autores.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
