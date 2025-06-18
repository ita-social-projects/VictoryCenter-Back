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
    private readonly VictoryCenterDbContext _dbContext;

    protected RepositoryBase(VictoryCenterDbContext context)
    {
        _dbContext = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync(QueryOptions<T>? queryOptions = null)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        if (queryOptions is not null)
        {
            query = ApplyFilter(query, queryOptions.FilterPredicate);
            query = ApplyInclude(query, queryOptions.Include);
        }

        return await query.ToListAsync();
    }

    public async Task<T?> GetFirstOrDefaultAsync(QueryOptions<T>? queryOptions = null)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        if (queryOptions is not null)
        {
            query = ApplyFilter(query, queryOptions.FilterPredicate);
            query = ApplyInclude(query, queryOptions.Include);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<T> CreateAsync(T entity)
    {
        var tmp = await _dbContext.Set<T>().AddAsync(entity);
        return tmp.Entity;
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

        if (!await projected.AnyAsync())
        {
            return null;
        }

        return await projected.MaxAsync();
    }

    private IQueryable<T> ApplyFilter(IQueryable<T> query, Expression<Func<T, bool>>? filter)
    {
        return filter is not null ? query.Where(filter) : query;
    }

    private IQueryable<T> ApplyInclude(IQueryable<T> query, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include)
    {
        return include is not null ? include(query) : query;
    }
}
