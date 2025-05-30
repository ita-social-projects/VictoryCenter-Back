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
        var query = GetQueryable(predicate);
        return await query.ToListAsync();
    }
    
    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>>? predicate = default)
    {
        var query = GetQueryable(predicate);
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

    private IQueryable<T> GetQueryable(Expression<Func<T, bool>>? predicate = default)
    {
        var query = _dbContext.Set<T>().AsNoTracking();
        
        return predicate is not null ? query.Where(predicate) : query.AsNoTracking();
    }
}
