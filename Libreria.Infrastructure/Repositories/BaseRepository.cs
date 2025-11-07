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
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _entities;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return _entities.AsEnumerable();
        }

        public async Task<T?> GetById(int id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _entities.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _entities.Update(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetById(id);
            if (entity != null)
                _entities.Remove(entity);
        }
    }
}
