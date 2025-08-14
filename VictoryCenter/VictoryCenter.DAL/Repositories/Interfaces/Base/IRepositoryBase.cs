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

    Task CreateRangeAsync(params T[] entities);

    Task CreateRangeAsync(IEnumerable<T> entities);

    EntityEntry<T> Update(T entity);

    void UpdateRange(params T[] entities);

    void UpdateRange(IEnumerable<T> entities);

    void Delete(T entity);

    void DeleteRange(params T[] entities);

    void DeleteRange(IEnumerable<T> entities);

    Task<TKey?> MaxAsync<TKey>(Expression<Func<T, TKey>> selector, Expression<Func<T, bool>>? filter = null)
        where TKey : struct;

    Task<long> CountAsync(Expression<Func<T, bool>> filter);
}
