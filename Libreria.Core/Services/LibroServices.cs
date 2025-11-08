using Dapper;
using Libreria.Core.CustomEntities;
using Libreria.Core.Entities;
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

        public LibroService(IUnitOfWork unitOfWork, IDapperContext dapper)
        {
            _unitOfWork = unitOfWork;
            _dapper = dapper;
        }

        // =====================================================
        // MÉTODOS CRUD BASE (sin cambios)
        // =====================================================
        public IEnumerable<Libro> GetAll()
        {
            var task = GetAllAsync();
            task.Wait();
            return task.Result;
        }

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
        // FILTROS CON PAGINACIÓN + ESTANDARIZACIÓN DE RESPUESTA
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

            // === Conteo total ===
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS TotalCountQuery";
            var totalCount = await _dapper.ExecuteScalarAsync<int>(countSql, parameters);

            // === Paginación ===
            var offset = (filters.PageNumber - 1) * filters.PageSize;
            sql += " ORDER BY l.Id LIMIT @PageSize OFFSET @Offset";
            parameters.Add("@PageSize", filters.PageSize);
            parameters.Add("@Offset", offset);

            var items = await _dapper.QueryAsync<Libro>(sql, parameters);
            var pagedList = new PagedList<object>(items.Cast<object>().ToList(), totalCount, filters.PageNumber, filters.PageSize);

            if (items.Any())
            {
                return new ResponseData
                {
                    Pagination = pagedList,
                    Messages = new[]
                    {
                        new Message
                        {
                            Type = TypeMessage.information.ToString(),
                            Description = "Libros recuperados correctamente."
                        }
                    },
                    StatusCode = HttpStatusCode.OK
                };
            }

            return new ResponseData
            {
                Pagination = pagedList,
                Messages = new[]
                {
                    new Message
                    {
                        Type = TypeMessage.warning.ToString(),
                        Description = "No se encontraron libros con los filtros aplicados."
                    }
                },
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}
