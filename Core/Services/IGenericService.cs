using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IGenericService<TEntity, TDto> where TEntity : class where TDto : class
    {
        Task<Response<TDto>> GetByIdAsync(int id);
        Task<Response<IEnumerable<TDto>>> GetAllAsync(); // Başka bir where sorgusu yapılmaz. Dataları direkt döndürür.
        Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate); // ToList yazana kadar istediğin kadar predicate yaz
        Task<Response<TDto>> AddAsync(TDto entity);
        Task<Response<NoDataDto>> Remove(int id);
        Task<Response<NoDataDto>> Update(TDto entity,int id);
    }
}
