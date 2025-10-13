using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Libreria.Core.Entities;
using Libreria.Core.Interfaces;

namespace Libreria.Core.Services
{
    public class LibroService
    {
        private readonly ILibroRepository _libroRepository;
        private readonly IAutorRepository _autorRepository;

        public LibroService(
            ILibroRepository libroRepository,
            IAutorRepository autorRepository)
        {
            _libroRepository = libroRepository;
            _autorRepository = autorRepository;
        }

        // Ejemplo de método con lógica de negocio
        public async Task<IEnumerable<Libro>> GetAllAsync()
        {
            return await _libroRepository.GetAllAsync();
        }

        public async Task<Libro?> GetByIdAsync(int id)
        {
            return await _libroRepository.GetByIdAsync(id);
        }

        public async Task InsertAsync(Libro libro)
        {
            // Validar si el autor existe
            var autor = await _autorRepository.GetByIdAsync(libro.AutorId);
            if (autor == null)
                throw new Exception("El autor especificado no existe.");

            await _libroRepository.AddAsync(libro);
        }

        public async Task UpdateAsync(Libro libro)
        {
            await _libroRepository.UpdateAsync(libro);
        }

        public async Task DeleteAsync(int id)
        {
            var libro = await _libroRepository.GetByIdAsync(id);
            if (libro == null)
                throw new Exception("El libro no existe.");

            await _libroRepository.DeleteAsync(libro);
        }
    }
}
