using Dapper;
using Libreria.Core.CustomEntities;
using Libreria.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Libreria.Core.Enums;
using Libreria.Core.Exceptions;
using Libreria.Core.Interfaces;
using Libreria.Core.QueryFilters;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Libreria.Core.Services
{
    public class LibroService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDapperContext _dapper;
        private readonly IDbConnectionFactory _connFactory;

        public LibroService(IUnitOfWork unitOfWork, IDapperContext dapper, IDbConnectionFactory connFactory)
        {
            _unitOfWork = unitOfWork;
            _dapper = dapper;
            _connFactory = connFactory;
        }

        // =====================================================
        // GET ALL (Dapper)
        // =====================================================
        public async Task<IEnumerable<Libro>> GetAllAsync()
        {
            using var connection = _connFactory.CreateConnection();

            var sql = @"
                SELECT 
                    l.Id, 
                    l.Titulo, 
                    l.AnioPublicacion, 
                    l.Descripcion, 
                    l.Precio, 
                    l.Stock, 
                    l.AutorId,
                    a.Id AS AutorSplit,
                    a.Nombre,
                    a.Apellido
                FROM Libros l
                INNER JOIN Autores a ON l.AutorId = a.Id;
            ";

            var libros = await connection.QueryAsync<Libro, Autor, Libro>(
                sql,
                (libro, autor) =>
                {
                    libro.Autor = autor;
                    return libro;
                },
                splitOn: "AutorSplit"
            );

            if (!libros.Any())
                throw new NotFoundException("No se encontraron libros registrados.");

            return libros;
        }

        // =====================================================
        // GET BY ID (Dapper)
        // =====================================================
        public async Task<Libro?> GetByIdAsync(int id)
        {
            using var connection = _connFactory.CreateConnection();

            var sql = @"
                SELECT 
                    l.Id, 
                    l.Titulo, 
                    l.AnioPublicacion, 
                    l.Descripcion, 
                    l.Precio, 
                    l.Stock, 
                    l.AutorId,
                    a.Id AS AutorSplit,
                    a.Nombre,
                    a.Apellido
                FROM Libros l
                LEFT JOIN Autores a ON l.AutorId = a.Id
                WHERE l.Id = @id;
            ";

            var result = await connection.QueryAsync<Libro, Autor, Libro>(
                sql,
                (libro, autor) =>
                {
                    libro.Autor = autor;
                    return libro;
                },
                new { id },
                splitOn: "AutorSplit"
            );

            var libro = result.FirstOrDefault();

            if (libro == null)
                throw new NotFoundException($"No se encontró el libro con ID {id}.");

            return libro;
        }

        // =====================================================
        // POST
        // =====================================================
        public async Task AddAsync(Libro libro)
        {
            if (string.IsNullOrWhiteSpace(libro.Titulo))
                throw new DomainValidationException("El título es obligatorio.");

            if (libro.Precio <= 0)
                throw new BusinessRuleException("El precio debe ser mayor a cero.");

            await _unitOfWork.Libros.Add(libro);
            await _unitOfWork.SaveChangesAsync();
        }

        // =====================================================
        // UPDATE
        // =====================================================
        public async Task UpdateAsync(Libro libro)
        {
            if (libro.Id <= 0)
                throw new DomainValidationException("Debe especificar un ID válido.");

            if (string.IsNullOrWhiteSpace(libro.Titulo))
                throw new DomainValidationException("El título es obligatorio.");

            if (libro.Precio <= 0)
                throw new BusinessRuleException("El precio debe ser mayor a cero.");

            var existing = await _unitOfWork.Libros.GetById(libro.Id);
            if (existing == null)
                throw new NotFoundException($"No existe un libro con ID {libro.Id}.");

            _unitOfWork.Libros.Update(libro);
            await _unitOfWork.SaveChangesAsync();
        }

        // =====================================================
        // DELETE
        // =====================================================
        public async Task DeleteAsync(int id)
        {
            var libro = await _unitOfWork.Libros.GetById(id);

            if (libro == null)
                throw new NotFoundException($"No existe un libro con ID {id}.");

            await _unitOfWork.Libros.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        // =====================================================
        // GET FILTERED + PAGINATION
        // =====================================================
        public async Task<ResponseData> GetFilteredAsync(LibroQueryFilter filters)
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

            // Total
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS CountQuery";
            var totalCount = await _dapper.ExecuteScalarAsync<int>(countSql, parameters);

            // Pagination
            var offset = (filters.PageNumber - 1) * filters.PageSize;
            sql += " ORDER BY l.Id LIMIT @PageSize OFFSET @Offset";

            parameters.Add("@PageSize", filters.PageSize);
            parameters.Add("@Offset", offset);

            var items = await _dapper.QueryAsync<Libro>(sql, parameters);

            // CONVERTIR A PagedList<object>
            var pagedList = new PagedList<object>(
                items.Cast<object>().ToList(),
                totalCount,
                filters.PageNumber,
                filters.PageSize
            );

            return new ResponseData
            {
                Pagination = pagedList,
                Messages = new[]
                {
            new Message
            {
                Type = items.Any()
                    ? TypeMessage.information.ToString()
                    : TypeMessage.warning.ToString(),
                Description = items.Any()
                    ? "Libros recuperados correctamente."
                    : "No se encontraron resultados."
            }
        },
                StatusCode = HttpStatusCode.OK
            };
        }

    }
}
