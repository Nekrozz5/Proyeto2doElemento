using System.Collections.Generic;
using System.Threading.Tasks;
using Libreria.Core.Entities;

namespace Libreria.Core.Interfaces
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        Task<T?> GetById(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        Task DeleteAsync(int id);
    }
}
