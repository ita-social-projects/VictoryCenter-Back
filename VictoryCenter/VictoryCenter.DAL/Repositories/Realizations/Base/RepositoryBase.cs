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
    protected readonly VictoryCenterDbContext _dbContext;

    protected RepositoryBase(VictoryCenterDbContext context)
    {
        _dbContext = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync(QueryOptions<T>? queryOptions = null)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        if (queryOptions != null)
        {
            query = ApplyInclude(query, queryOptions.Include);
            query = ApplyFilter(query, queryOptions.Filter);
            query = ApplyOrdering(query, queryOptions.OrderByASC, queryOptions.OrderByDESC);
            query = ApplyPagination(query, queryOptions.Offset, queryOptions.Limit);
            query = ApplySelector(query, queryOptions.Selector);
        }

        return await query.ToListAsync();
    }

    public async Task<T?> GetFirstOrDefaultAsync(QueryOptions<T>? queryOptions = null)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        if (queryOptions != null)
        {
            query = ApplyInclude(query, queryOptions.Include);
            query = ApplyFilter(query, queryOptions.Filter);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<T> CreateAsync(T entity)
    {
        var tmp = await _dbContext.Set<T>().AddAsync(entity);
        return tmp.Entity;
    }

    public async Task CreateRangeAsync(params T[] entities)
    {
        await _dbContext.Set<T>().AddRangeAsync(entities);
    }

    public async Task CreateRangeAsync(IEnumerable<T> entities)
    {
        await _dbContext.Set<T>().AddRangeAsync(entities);
    }

    public EntityEntry<T> Update(T entity)
    {
        return _dbContext.Set<T>().Update(entity);
    }

    public void UpdateRange(params T[] entities)
    {
        _dbContext.Set<T>().UpdateRange(entities);
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().UpdateRange(entities);
    }

    public void Delete(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
    }

    public void DeleteRange(params T[] entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
    }

    public async Task<TKey?> MaxAsync<TKey>(
        Expression<Func<T, TKey>> selector,
        Expression<Func<T, bool>>? filter = null)
        where TKey : struct
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        var projected = query.Select(selector);

        return await projected.DefaultIfEmpty().MaxAsync();
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

    private static IQueryable<T> ApplySelector(IQueryable<T> query, Expression<Func<T, T>>? selector)
    {
        return selector != null ? query.Select(selector) : query;
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
