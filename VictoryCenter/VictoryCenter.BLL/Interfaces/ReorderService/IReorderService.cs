using System.Linq.Expressions;
using VictoryCenter.DAL.Entities.Interfaces;

namespace VictoryCenter.BLL.Interfaces.ReorderService;

public interface IReorderService
{
    Task SwapElements<TEntity>(
        List<long> idsOrder,
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity;

    Task<long> GetNextDisplayOrder<TEntity>(
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity;

    Task RenumberPriorityAsync<TEntity>(
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity;
}

/*public interface IReorderService
{
    Task MoveElement<TEntity>(
        long elementId,
        long? prevElementId,
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, ILinkOrderableEntity;

    Task<TEntity?> GetLastElement<TEntity>(
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, ILinkOrderableEntity;

    Task<PaginationResult<TEntity>> GetOrderedPageAsync<TEntity>(
        int offset,
        int limit,
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>>? groupSelector = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        where TEntity : class, ILinkOrderableEntity;

    Task RemoveLinksFromListAsync<TEntity>(
        long elementId,
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, ILinkOrderableEntity;

    Task AppendToGroupEndAsync<TEntity>(
        long elementId,
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>> groupSelector)
        where TEntity : class, ILinkOrderableEntity;
}*/
