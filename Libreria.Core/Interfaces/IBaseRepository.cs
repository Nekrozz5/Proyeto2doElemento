using Libreria.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Libreria.Core.Interfaces
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        IQueryable<T> Query();  

        Task<T?> GetById(int id);
        Task Add(T entity);
        void Update(T entity);
        Task Delete(int id);
    }
}
