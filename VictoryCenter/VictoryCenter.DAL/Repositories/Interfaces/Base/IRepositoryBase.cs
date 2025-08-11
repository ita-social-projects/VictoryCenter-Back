using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.DAL.Repositories.Interfaces.Base;

public interface IRepositoryBase<T>
    where T : class
{
    Task<IEnumerable<T>> GetAllAsync(QueryOptions<T>? queryOptions = null);

    Task<T?> GetFirstOrDefaultAsync(QueryOptions<T>? queryOptions = null);

    Task<T> CreateAsync(T entity);

    EntityEntry<T> Update(T entity);

    void Delete(T entity);

    Task<int> CountAsync(QueryOptions<T>? queryOptions = null);

    Task<TKey?> MaxAsync<TKey>(Expression<Func<T, TKey>> selector, Expression<Func<T, bool>>? filter = null)
        where TKey : struct;
}
