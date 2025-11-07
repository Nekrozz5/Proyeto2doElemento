using Libreria.Core.Entities;
using Libreria.Core.Exceptions;
using Libreria.Core.Interfaces;
using Libreria.Core.QueryFilters;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var libros = _unitOfWork.Libros.Query()
                .Include(l => l.Autor)
                .AsNoTracking()
                .ToList();

            if (libros == null || !libros.Any())
                throw new NotFoundException("No se encontraron libros registrados.");

            return libros;
        }

        public async Task<Libro?> GetByIdAsync(int id)
        {
            var libro = await _unitOfWork.Libros.GetById(id);

            if (libro == null)
                throw new NotFoundException($"No se encontró el libro con ID {id}.");

            return libro;
        }

        public async Task AddAsync(Libro libro)
        {
            if (string.IsNullOrWhiteSpace(libro.Titulo))
                throw new DomainValidationException("El título del libro es obligatorio.");

            if (libro.Precio <= 0)
                throw new BusinessRuleException("El precio del libro debe ser mayor que cero.");

            await _unitOfWork.Libros.Add(libro);
            await _unitOfWork.SaveChangesAsync();
        }

        public void Update(Libro libro)
        {
            if (libro.Id <= 0)
                throw new DomainValidationException("Debe especificar un ID válido para actualizar.");

            if (string.IsNullOrWhiteSpace(libro.Titulo))
                throw new DomainValidationException("El título del libro es obligatorio.");

            if (libro.Precio <= 0)
                throw new BusinessRuleException("El precio del libro debe ser mayor que cero.");

            var existing = _unitOfWork.Libros.GetById(libro.Id).Result;
            if (existing == null)
                throw new NotFoundException($"No se puede actualizar: el libro con ID {libro.Id} no existe.");

            _unitOfWork.Libros.Update(libro);
            _unitOfWork.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            var libro = await _unitOfWork.Libros.GetById(id);

            if (libro == null)
                throw new NotFoundException($"No se puede eliminar: el libro con ID {id} no existe.");

            await _unitOfWork.Libros.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        // ==============================
        // QUERY FILTERS: Filtrar libros
        // ==============================
        public async Task<IEnumerable<Libro>> GetFilteredAsync(LibroQueryFilter filters)
        {
            var query = _unitOfWork.Libros.Query()
                .Include(l => l.Autor)
                .AsNoTracking()
                .AsQueryable();

            // Filtros dinámicos
            if (!string.IsNullOrWhiteSpace(filters.Titulo))
                query = query.Where(l => l.Titulo.Contains(filters.Titulo));

            if (!string.IsNullOrWhiteSpace(filters.Autor))
                query = query.Where(l => l.Autor.Nombre.Contains(filters.Autor));

            if (filters.MinPrecio.HasValue)
                query = query.Where(l => l.Precio >= filters.MinPrecio.Value);

            if (filters.MaxPrecio.HasValue)
                query = query.Where(l => l.Precio <= filters.MaxPrecio.Value);

            if (filters.Disponibles == true)
                query = query.Where(l => l.Stock > 0);

            var result = await query.ToListAsync();

            if (!result.Any())
                throw new NotFoundException("No se encontraron libros con los filtros aplicados.");

            return result;
        }
    }
}
    

