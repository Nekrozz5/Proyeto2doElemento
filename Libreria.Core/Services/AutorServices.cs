using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Libreria.Core.Entities;
using Libreria.Core.Interfaces;

namespace Libreria.Core.Services
{
    public class AutorService
    {
        private readonly IAutorRepository _autorRepository;

        public AutorService(IAutorRepository autorRepository)
        {
            _autorRepository = autorRepository;
        }

        public async Task<IEnumerable<Autor>> GetAllAsync()
        {
            return await _autorRepository.GetAllAsync();
        }

        public async Task<Autor?> GetByIdAsync(int id)
        {
            return await _autorRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(Autor autor)
        {
            // Evita autores duplicados
            var existentes = await _autorRepository.GetAllAsync();
            if (existentes.Any(a => a.Nombre == autor.Nombre && a.Apellido == autor.Apellido))
                throw new Exception("Ya existe un autor con el mismo nombre y apellido.");

            await _autorRepository.AddAsync(autor);
        }

        public async Task UpdateAsync(Autor autor)
        {
            await _autorRepository.UpdateAsync(autor);
        }

        public async Task DeleteAsync(int id)
        {
            var autor = await _autorRepository.GetByIdAsync(id);
            if (autor == null)
                throw new Exception("El autor no existe.");

            await _autorRepository.DeleteAsync(autor);
        }
    }
}
