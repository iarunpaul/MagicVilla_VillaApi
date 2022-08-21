using MagicVilla_VillaApi.Models;
using System.Linq.Expressions;

namespace MagicVilla_VillaApi.Repository.IRepository
{
    public interface IVillaRepository
    {
        Task CreateAsync(Villa entity);
        Task UpdateAsync(Villa entity);
        Task<List<Villa>> GetAllAsync(Expression<Func<Villa, bool>> filter = null);
        Task SaveAsync();
        Task<Villa> GetAsync(Expression<Func<Villa, bool>> filter = null, bool tracked=true);
        Task RemoveAsync(Villa entity);
    }
}
