using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;

namespace VictoryCenter.DAL.Repositories.Interfaces.Base;

public interface IRepositoryBase<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = default);
    
    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>>? predicate = default);
    
    Task<T> CreateAsync(T entity);
    
    EntityEntry<T> Update(T entity);
    
    void Delete(T entity);

    Task<TKey> MaxAsync<TKey>(Expression<Func<T, TKey>> selector, Expression<Func<T, bool>>? filter = null) where TKey : struct;
}
