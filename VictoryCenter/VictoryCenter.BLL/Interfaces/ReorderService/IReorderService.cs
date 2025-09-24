using System.Linq.Expressions;
using VictoryCenter.DAL.Entities.Interfaces;

namespace VictoryCenter.BLL.Interfaces.ReorderService;

public interface IReorderService
{
    Task SwapElementsAsync<TEntity>(
        List<long> idsOrder,
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity;

    Task<long> GetNextDisplayOrderAsync<TEntity>(
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity;

    Task RenumberPriorityAsync<TEntity>(
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity;
}
