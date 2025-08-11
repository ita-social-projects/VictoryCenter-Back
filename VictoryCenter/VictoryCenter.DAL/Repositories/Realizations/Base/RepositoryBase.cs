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
        IQueryable<T> query = _dbContext.Set<T>();
        query = ApplyTracking(query, queryOptions?.AsNoTracking ?? true);

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
        IQueryable<T> query = _dbContext.Set<T>();
        query = ApplyTracking(query, queryOptions?.AsNoTracking ?? true);

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

    public async Task<int> CountAsync(QueryOptions<T>? queryOptions = null)
    {
        IQueryable<T> query = _dbContext.Set<T>();

        if (queryOptions != null)
        {
            query = ApplyFilter(query, queryOptions.Filter);
        }

        return await query.CountAsync();
    }

    public EntityEntry<T> Update(T entity)
    {
        return _dbContext.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
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

    private IQueryable<T> ApplyTracking(IQueryable<T> query, bool asNoTracking)
    {
        return asNoTracking ? query.AsNoTracking() : query;
    }
}
