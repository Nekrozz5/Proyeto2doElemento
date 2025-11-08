using Libreria.Core.Entities;
using Libreria.Core.Interfaces;
using Libreria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Libreria.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        // Contexto de base de datos
        protected readonly ApplicationDbContext _context;

        // Conjunto de entidades del tipo genérico T
        protected readonly DbSet<T> _entities;

        // Constructor: recibe el contexto por inyección
        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }
        public IQueryable<T> Query()
        {
            return _entities.AsQueryable();
        }

        // Obtener todos los registros
        public IEnumerable<T> GetAll()
        {
            return _entities.AsEnumerable();
        }

        // Obtener un registro por ID
        public async Task<T?> GetById(int id)
        {
            return await _entities.FindAsync(id);
        }

        // Agregar un nuevo registro (sin guardar aún)
        public async Task Add(T entity)
        {
            await _entities.AddAsync(entity);
        }

        // Actualizar un registro existente
        public void Update(T entity)
        {
            var local = _entities.Local.FirstOrDefault(e => e.Id == entity.Id);
            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }
            _entities.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        // Eliminar un registro por ID (sin guardar aún)
        public async Task Delete(int id)
        {
            var entity = await GetById(id);
            if (entity != null)
            {
                _entities.Remove(entity);
            }
        }
    }
}
