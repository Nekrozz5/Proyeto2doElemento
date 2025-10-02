using Libreria.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libreria.Core.Interfaces
{
    public interface ILibroRepository
    {
        Task<IEnumerable<Libro>> GetAllAsync();
        Task<Libro?> GetByIdAsync(int id);
        Task AddAsync(Libro libro);
        Task UpdateAsync(Libro libro);
        Task DeleteAsync(Libro libro);

        // Método adicional: buscar libros por Autor
        Task<IEnumerable<Libro>> GetLibrosPorAutorAsync(int autorId);
    }
}
