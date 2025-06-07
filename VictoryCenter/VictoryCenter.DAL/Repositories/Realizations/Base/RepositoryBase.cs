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
    
    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = default)
    {
        var query = predicate is not null ? GetFiltered(predicate) : _dbContext.Set<T>().AsNoTracking();
        return await query.ToListAsync();
    }
    
    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>>? predicate = default)
    {
        var query = predicate is not null ? GetFiltered(predicate) : _dbContext.Set<T>().AsNoTracking();
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

    protected IQueryable<T> GetQueryable => _dbContext.Set<T>().AsQueryable();

    protected IQueryable<T> GetFiltered(Expression<Func<T, bool>> predicate)
    {
        var query = _dbContext.Set<T>().AsNoTracking();
        
        return query.Where(predicate);
    }

    protected IQueryable<T> GetPaginated(int offset, int limit)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        return query.Skip(offset).Take(limit);
    }

    protected IQueryable<T> GetOrderedByAscending(Expression<Func<T, object>> keySelector)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        return query.OrderBy(keySelector);
    }

    protected IQueryable<T> GetOrderedByDescending(Expression<Func<T, object>> keySelector)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        return query.OrderByDescending(keySelector);
    }

    protected IQueryable<T> GetSelected(Expression<Func<T, T>> selector)
    {
        var query = _dbContext.Set<T>().AsNoTracking();

        return query.Select(selector);
    }
}
