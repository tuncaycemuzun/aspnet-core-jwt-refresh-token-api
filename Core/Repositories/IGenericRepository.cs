using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync(); // Başka bir where sorgusu yapılmaz. Dataları direkt döndürür.
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate); // ToList yazana kadar istediğin kadar predicate yaz
        Task AddAsync(TEntity entity);
        void Remove(TEntity entity);
        TEntity Update(TEntity entity);

    }
}
