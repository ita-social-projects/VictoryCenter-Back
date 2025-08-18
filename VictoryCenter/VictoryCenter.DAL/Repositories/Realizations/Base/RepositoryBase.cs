using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.DAL.Repositories.Realizations.Base;

public class RepositoryBase<T> : IRepositoryBase<T>
    where T : class
{
    protected RepositoryBase(VictoryCenterDbContext context)
    {
        DbContext = context;
    }

    protected VictoryCenterDbContext DbContext { get; init; }

    public async Task<IEnumerable<T>> GetAllAsync(QueryOptions<T>? queryOptions = null)
    {
        var query = DbContext.Set<T>().AsNoTracking();

        if (queryOptions != null)
        {
            query = ApplyInclude(query, queryOptions.Include);
            query = ApplyFilter(query, queryOptions.Filter);
            query = ApplyOrdering(query, queryOptions.OrderByASC, queryOptions.OrderByDESC);
            query = ApplyPagination(query, queryOptions.Offset, queryOptions.Limit);
        }

        return await query.ToListAsync();
    }

    public async Task<T?> GetFirstOrDefaultAsync(QueryOptions<T>? queryOptions = null)
    {
        var query = DbContext.Set<T>().AsNoTracking();

        if (queryOptions != null)
        {
            query = ApplyInclude(query, queryOptions.Include);
            query = ApplyFilter(query, queryOptions.Filter);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<T> CreateAsync(T entity)
    {
        var tmp = await DbContext.Set<T>().AddAsync(entity);
        return tmp.Entity;
    }

    public async Task CreateRangeAsync(params T[] entities)
    {
        await DbContext.Set<T>().AddRangeAsync(entities);
    }

    public async Task CreateRangeAsync(IEnumerable<T> entities)
    {
        await DbContext.Set<T>().AddRangeAsync(entities);
    }

    public EntityEntry<T> Update(T entity)
    {
        return DbContext.Set<T>().Update(entity);
    }

    public void UpdateRange(params T[] entities)
    {
        DbContext.Set<T>().UpdateRange(entities);
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        DbContext.Set<T>().UpdateRange(entities);
    }

    public void Delete(T entity)
    {
        DbContext.Set<T>().Remove(entity);
    }

    public void DeleteRange(params T[] entities)
    {
        DbContext.Set<T>().RemoveRange(entities);
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        DbContext.Set<T>().RemoveRange(entities);
    }

    public async Task<TKey?> MaxAsync<TKey>(
        Expression<Func<T, TKey>> selector,
        Expression<Func<T, bool>>? filter = null)
        where TKey : struct
    {
        var query = DbContext.Set<T>().AsNoTracking();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        var projected = query.Select(selector);

        return await projected.DefaultIfEmpty().MaxAsync();
    }

    public Task<long> CountAsync(Expression<Func<T, bool>> filter)
    {
        return DbContext.Set<T>().LongCountAsync(filter);
    }

    private static IQueryable<T> ApplyFilter(IQueryable<T> query, Expression<Func<T, bool>>? filter)
    {
        return filter is not null ? query.Where(filter) : query;
    }

    private static IQueryable<T> ApplyInclude(IQueryable<T> query, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include)
    {
        return include is not null ? include(query) : query;
    }

    private static IQueryable<T> ApplyOrdering(
        IQueryable<T> query,
        Expression<Func<T, object>>? orderByASC,
        Expression<Func<T, object>>? orderByDESC)
    {
        if (orderByASC != null)
        {
            return query.OrderBy(orderByASC);
        }

        if (orderByDESC != null)
        {
            return query.OrderByDescending(orderByDESC);
        }

        return query;
    }

    private static IQueryable<T> ApplyPagination(IQueryable<T> query, int offset, int limit)
    {
        if (offset > 0)
        {
            query = query.Skip(offset);
        }

        if (limit > 0)
        {
            query = query.Take(limit);
        }

        return query;
    }
}
