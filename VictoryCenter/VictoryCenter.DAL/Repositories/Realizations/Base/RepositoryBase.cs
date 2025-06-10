using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Base;

public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    private readonly VictoryCenterDbContext _dbContext;

    protected RepositoryBase(VictoryCenterDbContext context)
    {
        _dbContext = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync(
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default,
        Expression<Func<T, bool>>? predicate = default,
        int offset = 0,
        int limit = 0,
        Expression<Func<T, object>>? orderByASC = default,
        Expression<Func<T, object>>? orderByDESC = default,
        Expression<Func<T, T>>? selector = default)
    {
        var query = _dbContext.Set<T>().AsNoTracking();
        query = ApplyInclude(query, include);
        query = ApplyPredicate(query, predicate);
        query = ApplyPagination(query, offset, limit);
        query = ApplyOrdering(query, orderByASC, orderByDESC);
        query = ApplySelector(query, selector);
        return await query.ToListAsync();
    }
    
    public async Task<T?> GetFirstOrDefaultAsync(
        Expression<Func<T, bool>>? predicate = default,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default)
    {
        var query = _dbContext.Set<T>().AsNoTracking();
        query = ApplyInclude(query, include);
        query = ApplyPredicate(query, predicate);
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

    private static IQueryable<T> ApplyPredicate(IQueryable<T> query, Expression<Func<T, bool>>? predicate)
    {
        return predicate != null ? query.Where(predicate) : query;
    }

    private static IQueryable<T> ApplyInclude(IQueryable<T> query, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include)
    {
        return include != null ? include(query) : query;
    }

    private static IQueryable<T> ApplyOrdering(
        IQueryable<T> query,
        Expression<Func<T, object>>? orderByASC,
        Expression<Func<T, object>>? orderByDESC)
    {
        if (orderByASC != null)
            return query.OrderBy(orderByASC);
        if (orderByDESC != null)
            return query.OrderByDescending(orderByDESC);

        return query;
    }

    private static IQueryable<T> ApplySelector(IQueryable<T> query, Expression<Func<T, T>>? selector)
    {
        return selector != null ? query.Select(selector) : query;
    }

    private static IQueryable<T> ApplyPagination(IQueryable<T> query, int offset, int limit)
    {
        if (offset > 0)
            query = query.Skip(offset);
        if (limit > 0)
            query = query.Take(limit);

        return query;
    }

}
