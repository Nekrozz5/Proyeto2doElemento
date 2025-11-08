using Dapper;
using Libreria.Core.Entities;
using Libreria.Core.Exceptions;
using Libreria.Core.Interfaces;
using Libreria.Core.QueryFilters;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Libreria.Core.CustomEntities;
using System.Threading.Tasks;

namespace Libreria.Core.Services
{
    public class AutorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDapperContext _dapper;

        public AutorService(IUnitOfWork unitOfWork, IDapperContext dapper)
        {
            _unitOfWork = unitOfWork;
            _dapper = dapper;
        }

        // ==========================================================
        // GET: Todos los autores (con solo Id y Titulo de sus libros)
        // ==========================================================
        public IEnumerable<object> GetAll()
        {
            var autores = _unitOfWork.Autores
                .Query()
                .Include(a => a.Libros)
                .AsNoTracking()
                .Select(a => new
                {
                    a.Id,
                    a.Nombre,
                    a.Apellido,
                    a.FechaRegistro,
                    a.FechaActualizacion,
                    Libros = a.Libros.Select(l => new
                    {
                        l.Id,
                        l.Titulo
                    })
                })
                .ToList();

            if (!autores.Any())
                throw new NotFoundException("No se encontraron autores registrados.");

            return autores;
        }

        // ==========================================================
        // GET: Autor por Id (con solo Id y Titulo de sus libros)
        // ==========================================================
        public async Task<object?> GetByIdAsync(int id)
        {
            var autor = await _unitOfWork.Autores
                .Query()
                .Include(a => a.Libros)
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new
                {
                    a.Id,
                    a.Nombre,
                    a.Apellido,
                    a.FechaRegistro,
                    a.FechaActualizacion,
                    Libros = a.Libros.Select(l => new
                    {
                        l.Id,
                        l.Titulo
                    })
                })
                .FirstOrDefaultAsync();

            if (autor == null)
                throw new NotFoundException($"No se encontró el autor con ID {id}.");

            return autor;
        }

        // ==========================================================
        // POST: Crear nuevo autor
        // ==========================================================
        public async Task AddAsync(Autor autor)
        {
            if (string.IsNullOrWhiteSpace(autor.Nombre))
                throw new DomainValidationException("El nombre del autor es obligatorio.");

            await _unitOfWork.Autores.Add(autor);
            await _unitOfWork.SaveChangesAsync();
        }

        // ==========================================================
        // PUT: Actualizar autor
        // ==========================================================
        public void Update(Autor autor)
        {
            if (autor.Id <= 0)
                throw new DomainValidationException("Debe especificar un ID válido para actualizar.");

            var existing = _unitOfWork.Autores.GetById(autor.Id).Result;
            if (existing == null)
                throw new NotFoundException($"No se puede actualizar: el autor con ID {autor.Id} no existe.");

            _unitOfWork.Autores.Update(autor);
            _unitOfWork.SaveChanges();
        }

        // ==========================================================
        // DELETE: Eliminar autor
        // ==========================================================
        public async Task DeleteAsync(int id)
        {
            var autor = await _unitOfWork.Autores.GetById(id);
            if (autor == null)
                throw new NotFoundException($"No se puede eliminar: el autor con ID {id} no existe.");

            await _unitOfWork.Autores.Delete(id);
            await _unitOfWork.SaveChangesAsync();
        }

        // ==========================================================
        // FILTROS Y RESUMEN (Dapper)
        // ==========================================================
        public async Task<PagedList<Autor>> GetFilteredAsync(AutorQueryFilter filters)
        {
            var sql = @"SELECT 
                    a.Id, a.Nombre, a.Apellido,
                    COUNT(l.Id) AS LibrosPublicados
                FROM Autores a
                LEFT JOIN Libros l ON a.Id = l.AutorId
                WHERE 1=1 ";
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(filters.Nombre))
            {
                sql += " AND LOWER(a.Nombre) LIKE CONCAT('%', LOWER(@Nombre), '%')";
                parameters.Add("@Nombre", filters.Nombre);
            }

            if (!string.IsNullOrWhiteSpace(filters.Apellido))
            {
                sql += " AND LOWER(a.Apellido) LIKE CONCAT('%', LOWER(@Apellido), '%')";
                parameters.Add("@Apellido", filters.Apellido);
            }

            var countSql = $"SELECT COUNT(*) FROM ({sql} GROUP BY a.Id) AS CountQuery";
            var totalCount = await _dapper.ExecuteScalarAsync<int>(countSql, parameters);

            var offset = (filters.PageNumber - 1) * filters.PageSize;
            sql += " GROUP BY a.Id ORDER BY a.Nombre LIMIT @PageSize OFFSET @Offset";
            parameters.Add("@PageSize", filters.PageSize);
            parameters.Add("@Offset", offset);

            var items = await _dapper.QueryAsync<Autor>(sql, parameters);
            return new PagedList<Autor>(items.ToList(), totalCount, filters.PageNumber, filters.PageSize);
        }

        public async Task<IEnumerable<dynamic>> GetResumenAsync()
        {
            var sql = @"SELECT 
                            a.Id, 
                            CONCAT(a.Nombre, ' ', a.Apellido) AS AutorNombre,
                            COUNT(l.Id) AS LibrosPublicados
                        FROM Autores a
                        LEFT JOIN Libros l ON a.Id = l.AutorId
                        GROUP BY a.Id, a.Nombre, a.Apellido
                        ORDER BY a.Nombre;";

            return await _dapper.QueryAsync<dynamic>(sql);
        }
    }
}
