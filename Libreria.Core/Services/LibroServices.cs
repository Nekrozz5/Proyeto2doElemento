using Dapper; // ✅ Necesario para DynamicParameters
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
        private readonly IDapperContext _dapper; // ✅ Dapper inyectado

        public LibroService(IUnitOfWork unitOfWork, IDapperContext dapper)
        {
            _unitOfWork = unitOfWork;
            _dapper = dapper;
        }

        // =====================================================
        // MÉTODO USADO POR EL CONTROLLER (sin cambiar la firma)
        // =====================================================
        public IEnumerable<Libro> GetAll()
        {
            // Dapper no tiene métodos síncronos → llamamos un método async internamente.
            var task = GetAllAsync();
            task.Wait(); // Esperar resultado (sin alterar el controller)
            return task.Result;
        }

        // =====================================================
        // Implementación interna con Dapper
        // =====================================================
        private async Task<IEnumerable<Libro>> GetAllAsync()
        {
            var sql = @"SELECT l.Id, l.Titulo, l.Precio, l.Stock, 
                               l.AutorId, a.Nombre AS AutorNombre, a.Apellido AS AutorApellido
                        FROM Libros l
                        LEFT JOIN Autores a ON l.AutorId = a.Id";

            var libros = await _dapper.QueryAsync<Libro>(sql);

            if (libros == null || !libros.Any())
                throw new NotFoundException("No se encontraron libros registrados.");

            return libros;
        }

        // =====================================================
        // GET BY ID (Dapper también)
        // =====================================================
        public async Task<Libro?> GetByIdAsync(int id)
        {
            var sql = @"SELECT l.Id, l.Titulo, l.Precio, l.Stock, 
                               l.AutorId, a.Nombre AS AutorNombre, a.Apellido AS AutorApellido
                        FROM Libros l
                        LEFT JOIN Autores a ON l.AutorId = a.Id
                        WHERE l.Id = @id";

            var libro = await _dapper.QueryFirstOrDefaultAsync<Libro>(sql, new { id });

            if (libro == null)
                throw new NotFoundException($"No se encontró el libro con ID {id}.");

            return libro;
        }

        // =====================================================
        // CREATE / UPDATE / DELETE (EF CORE)
        // =====================================================
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

        // =====================================================
        // FILTROS (Dapper también)
        // =====================================================
        public async Task<IEnumerable<Libro>> GetFilteredAsync(LibroQueryFilter filters)
        {
            var sql = @"SELECT l.Id, l.Titulo, l.Precio, l.Stock, 
                               l.AutorId, a.Nombre AS AutorNombre, a.Apellido AS AutorApellido
                        FROM Libros l
                        LEFT JOIN Autores a ON l.AutorId = a.Id
                        WHERE 1=1 ";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(filters.Titulo))
            {
                sql += " AND LOWER(l.Titulo) LIKE CONCAT('%', LOWER(@Titulo), '%')";
                parameters.Add("@Titulo", filters.Titulo.Trim());
            }

            if (!string.IsNullOrWhiteSpace(filters.Autor))
            {
                sql += @" AND (
                            LOWER(a.Nombre) LIKE CONCAT('%', LOWER(@Autor), '%')
                            OR LOWER(a.Apellido) LIKE CONCAT('%', LOWER(@Autor), '%')
                            OR LOWER(CONCAT(a.Nombre, ' ', a.Apellido)) LIKE CONCAT('%', LOWER(@Autor), '%')
                          )";
                parameters.Add("@Autor", filters.Autor.Trim());
            }

            if (filters.MinPrecio.HasValue)
            {
                sql += " AND l.Precio >= @MinPrecio";
                parameters.Add("@MinPrecio", filters.MinPrecio.Value);
            }

            if (filters.MaxPrecio.HasValue)
            {
                sql += " AND l.Precio <= @MaxPrecio";
                parameters.Add("@MaxPrecio", filters.MaxPrecio.Value);
            }

            if (filters.Disponibles == true)
                sql += " AND l.Stock > 0";

            var result = await _dapper.QueryAsync<Libro>(sql, parameters);
            return result;
        }
    }
}
