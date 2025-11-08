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

            // Filtro por título
            if (!string.IsNullOrWhiteSpace(filters.Titulo))
                query = query.Where(l => l.Titulo.ToLower().Contains(filters.Titulo.Trim().ToLower()));

            // --- AUTOR: búsqueda robusta (nombre, apellido o nombre completo) ---
            if (!string.IsNullOrWhiteSpace(filters.Autor))
            {
                var term = filters.Autor.Trim().ToLower();

                query = query.Where(l =>
                    l.Autor != null && (
                        ((l.Autor.Nombre ?? "").ToLower().Contains(term)) ||
                        ((l.Autor.Apellido ?? "").ToLower().Contains(term)) ||
                        (((l.Autor.Nombre ?? "") + " " + (l.Autor.Apellido ?? "")).ToLower().Contains(term))
                    )
                );
            }

            // Rango de precios
            if (filters.MinPrecio.HasValue)
                query = query.Where(l => l.Precio >= filters.MinPrecio.Value);

            if (filters.MaxPrecio.HasValue)
                query = query.Where(l => l.Precio <= filters.MaxPrecio.Value);

            // Libros disponibles
            if (filters.Disponibles == true)
                query = query.Where(l => l.Stock > 0);

            var result = await query.ToListAsync();

            // Si no hay resultados, devolvemos lista vacía (no excepción)
            return result;
        }
    }
}
