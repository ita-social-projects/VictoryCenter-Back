using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Services.ReorderService;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Interfaces.ReorderService;

public interface IReorderService
{
    Task MoveElement<TEntity>(
        long elementId,
        long? prevElementId,
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity;

    Task<TEntity?> GetLastElement<TEntity>(
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity;

    Task<PaginationResult<TEntity>> GetOrderedPageAsync<TEntity>(
        int offset,
        int limit,
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>>? groupSelector = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        where TEntity : class, IOrderableEntity;

    Task RemoveLinksFromListAsync<TEntity>(
        long elementId,
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity;

    Task AppendToGroupEndAsync<TEntity>(
        long elementId,
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>> groupSelector)
        where TEntity : class, IOrderableEntity;
}

public interface IIndexReorderService
{
    Task<PaginationResult<TEntity>> GetOrderedPageAsync<TEntity>(
        int offset,
        int limit,
        Expression<Func<TEntity, bool>>? groupSelector = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        where TEntity : class, IIndexOrderableEntity;

    Task SwapElements<TEntity>(
        List<long> idsOrder,
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IIndexOrderableEntity;

    Task<long> GetNextPriority<TEntity>(
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IIndexOrderableEntity;

    Task RenumberPriorityAsync<TEntity>(
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IIndexOrderableEntity;
}

public class IndexReorderService : IIndexReorderService
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private static readonly bool ThrowErrors = false;

    public IndexReorderService(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<PaginationResult<TEntity>> GetOrderedPageAsync<TEntity>(
        int offset,
        int limit,
        Expression<Func<TEntity, bool>>? groupSelector = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        where TEntity : class, IIndexOrderableEntity
    {
        var repository = _repositoryWrapper.GetRepository<TEntity>();

        var items = await repository.GetAllAsync(new QueryOptions<TEntity>
        {
            Filter = groupSelector,
            OrderByASC = e => e.Priority,
            Offset = offset,
            Limit = limit,
            Include = include
        });

        var totalCount = await repository.CountAsync(new QueryOptions<TEntity>
        {
            Filter = groupSelector
        });

        return new PaginationResult<TEntity>(items.ToArray(), totalCount);
    }

    /*    public async Task SwapElements<TEntity>(
            List<long> idsOrder,
            Expression<Func<TEntity, long>> idSelector,
            Expression<Func<TEntity, bool>>? groupSelector = null)
            where TEntity : class, IIndexOrderableEntity
        {
            idsOrder = idsOrder.Distinct().ToList();

            if (idsOrder.Count == 0)
            {
                return;
            }

            var repository = _repositoryWrapper.GetRepository<TEntity>();
            var idSelectorCompiled = idSelector.Compile();

            var entities = (await repository.GetAllAsync(new QueryOptions<TEntity>
            {
                Filter = e => idsOrder.Contains(idSelectorCompiled(e)) && (groupSelector == null || groupSelector.Compile()(e)),
                OrderByASC = e => e.Priority,
                AsNoTracking = false
            })).ToList();

            if (entities.Count != idsOrder.Count)
            {
                throw new InvalidOperationException("Some entities were not found for the provided IDs.");
            }

            var oldPriorities = entities.Select(e => e.Priority).ToList();
            var idToEntityMap = entities.ToDictionary(idSelectorCompiled);

            for (int i = 0; i < idsOrder.Count; i++)
            {
                var currentId = idsOrder[i];
                var entityToUpdate = idToEntityMap[currentId];
                entityToUpdate.Priority = oldPriorities[i];
            }
        }*/

    /*    public async Task SwapElements<TEntity>(
            List<long> idsOrder,
            Expression<Func<TEntity, long>> idSelector,
            Expression<Func<TEntity, bool>>? groupSelector = null)
            where TEntity : class, IIndexOrderableEntity
        {
            if (idsOrder == null || !idsOrder.Any())
            {
                return;
            }

            var repository = _repositoryWrapper.GetRepository<TEntity>();

            // Будуємо правильний вираз для фільтрації, який може бути трансльований в SQL
            var idValues = idsOrder.Distinct().ToList();
            var parameter = idSelector.Parameters.Single();
            var containsMethod = typeof(List<long>).GetMethod("Contains", [typeof(long)]);
            var body = Expression.Call(Expression.Constant(idValues), containsMethod, idSelector.Body);
            var idFilter = Expression.Lambda<Func<TEntity, bool>>(body, parameter);

            var finalFilter = idFilter;
            if (groupSelector != null)
            {
                // Об'єднуємо фільтри по ID та по групі
                var invokedGroupSelector = Expression.Invoke(groupSelector, parameter);
                finalFilter = Expression.Lambda<Func<TEntity, bool>>(
                    Expression.AndAlso(idFilter.Body, invokedGroupSelector), parameter);
            }

            var entities = (await repository.GetAllAsync(new QueryOptions<TEntity>
            {
                Filter = finalFilter,
                AsNoTracking = false,
                OrderByASC = e => e.Priority
            })).ToList();

            if (entities.Count != idValues.Count)
            {
                throw new InvalidOperationException("Some entities were not found for the provided IDs or they belong to a different group.");
            }

            var idToEntityMap = entities.ToDictionary(idSelector.Compile());
            var oldPriorities = entities.Select(e => e.Priority).ToList();

            // Step 1: Temp priorities to avoid unique constraint issues (intermediate state)
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Priority = -i - 1;
                repository.Update(entities[i]);
            }

            await _repositoryWrapper.SaveChangesAsync();

            // Step 2: Assign new priorities based on idsOrder
            for (int i = 0; i < idValues.Count; i++)
            {
                var currentId = idValues[i];
                if (idToEntityMap.TryGetValue(currentId, out var entityToUpdate))
                {
                    entityToUpdate.Priority = oldPriorities[i];
                    repository.Update(entityToUpdate);
                }
            }

            await _repositoryWrapper.SaveChangesAsync();
        }*/

    /*    public async Task SwapElements<TEntity>(
            List<long> idsOrder,
            Expression<Func<TEntity, long>> idSelector,
            Expression<Func<TEntity, bool>>? groupSelector = null)
            where TEntity : class, IIndexOrderableEntity
        {
            if (idsOrder == null || !idsOrder.Any())
            {
                return;
            }

            var repository = _repositoryWrapper.GetRepository<TEntity>();
            var idSelectorCompiled = idSelector.Compile();
            idsOrder = idsOrder.Distinct().ToList();

            var entities = (await repository.GetAllAsync(new QueryOptions<TEntity>
            {
                Filter = groupSelector,
                AsNoTracking = false
            })).Where(e => idsOrder.Contains(idSelectorCompiled(e))).ToList();

            if (entities.Count != idsOrder.Count)
            {
                throw new ReorderException("Some entities were not found for the provided IDs or they belong to a different group.");
            }

            var idToEntityMap = entities.ToDictionary(idSelectorCompiled);
            var oldPriorities = entities.OrderBy(e => e.Priority).Select(e => e.Priority).ToList();

            // Step 1: Temp priorities to avoid unique constraint issues (intermediate state)
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Priority = -i - 1;
                repository.Update(entities[i]);

                if (i > 3 && ThrowErrors)
                {
                    throw new InvalidOperationException("Some unexpected error occured during temp priorities assign");
                }
            }

            if (ThrowErrors)
            {
                throw new InvalidOperationException("Some error occured before temp prioritie save");
            }

            await _repositoryWrapper.SaveChangesAsync();

            if (ThrowErrors)
            {
                throw new InvalidOperationException("Some error occured after temp prioritie save");
            }

            // Step 2: Assign new priorities based on idsOrder
            for (int i = 0; i < idsOrder.Count; i++)
            {
                var currentId = idsOrder[i];
                if (idToEntityMap.TryGetValue(currentId, out var entityToUpdate))
                {
                    entityToUpdate.Priority = oldPriorities[i];
                    repository.Update(entityToUpdate);
                }

                if (i > 4 && ThrowErrors)
                {
                    throw new InvalidOperationException("Some unexpected error occured during proper priorities assign");
                }
            }

            if (ThrowErrors)
            {
                throw new InvalidOperationException("Some error occured before proper prioritie save");
            }

            await _repositoryWrapper.SaveChangesAsync();

            if (ThrowErrors)
            {
                throw new InvalidOperationException("Some error occured after proper prioritie save");
            }
        }*/

    public async Task SwapElements<TEntity>(
    List<long> idsOrder,
    Expression<Func<TEntity, long>> idSelector,
    Expression<Func<TEntity, bool>>? groupSelector = null)
    where TEntity : class, IIndexOrderableEntity
    {
        if (idsOrder == null || !idsOrder.Any())
        {
            return;
        }

        var repository = _repositoryWrapper.GetRepository<TEntity>();
        var idSelectorCompiled = idSelector.Compile();
        idsOrder = idsOrder.Distinct().ToList();

        var entities = (await repository.GetAllAsync(new QueryOptions<TEntity>
        {
            Filter = groupSelector,
            AsNoTracking = false
        })).Where(e => idsOrder.Contains(idSelectorCompiled(e))).ToList();

        if (entities.Count != idsOrder.Count)
        {
            throw new ReorderException("Some entities were not found for the provided IDs or they belong to a different group.");
        }

        var idToEntityMap = entities.ToDictionary(idSelectorCompiled);
        var oldPriorities = entities.OrderBy(e => e.Priority).Select(e => e.Priority).ToList();

        // Step 1: Temp priorities to avoid unique constraint issues (intermediate state)
        for (int i = 0; i < entities.Count; i++)
        {
            entities[i].Priority = -i - 1;
            repository.Update(entities[i]);
        }

        await _repositoryWrapper.SaveChangesAsync();

        // Step 2: Assign new priorities based on idsOrder
        for (int i = 0; i < idsOrder.Count; i++)
        {
            var currentId = idsOrder[i];
            if (idToEntityMap.TryGetValue(currentId, out var entityToUpdate))
            {
                entityToUpdate.Priority = oldPriorities[i];
                repository.Update(entityToUpdate);
            }
        }

        await _repositoryWrapper.SaveChangesAsync();
    }

    /*    public async Task SwapElements<TEntity>(
    List<long> idsOrder,
    Expression<Func<TEntity, long>> idSelector,
    Expression<Func<TEntity, bool>>? groupSelector = null)
    where TEntity : class, IIndexOrderableEntity
        {
            if (idsOrder == null || !idsOrder.Any())
            {
                return;
            }

            var repository = _repositoryWrapper.GetRepository<TEntity>();
            var idSelectorCompiled = idSelector.Compile();
            idsOrder = idsOrder.Distinct().ToList();

            var entities = (await repository.GetAllAsync(new QueryOptions<TEntity>
            {
                Filter = groupSelector,
                AsNoTracking = false
            })).Where(e => idsOrder.Contains(idSelectorCompiled(e))).ToList();

            if (entities.Count != idsOrder.Count)
            {
                throw new ReorderException("Some entities were not found for the provided IDs or they belong to a different group.");
            }

            var idToEntityMap = entities.ToDictionary(idSelectorCompiled);
            var oldPriorities = entities.OrderBy(e => e.Priority).Select(e => e.Priority).ToList();

            for (int i = 0; i < idsOrder.Count; i++)
            {
                var currentId = idsOrder[i];
                if (idToEntityMap.TryGetValue(currentId, out var entityToUpdate))
                {
                    if (entityToUpdate.Priority != oldPriorities[i])
                    {
                        entityToUpdate.Priority = oldPriorities[i];
                        repository.Update(entityToUpdate);
                    }
                }
            }

            await _repositoryWrapper.SaveChangesAsync();
        }*/

    public async Task<long> GetNextPriority<TEntity>(
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IIndexOrderableEntity
    {
        var repository = _repositoryWrapper.GetRepository<TEntity>();
        var maxPriority = await repository.MaxAsync(e => e.Priority, groupSelector);
        return (maxPriority ?? 0) + 1;
    }

    public async Task RenumberPriorityAsync<TEntity>(
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IIndexOrderableEntity
    {
        var repository = _repositoryWrapper.GetRepository<TEntity>();
        var itemsToRenumber = (await repository.GetAllAsync(new QueryOptions<TEntity>
        {
            Filter = groupSelector,
            OrderByASC = e => e.Priority,
            AsNoTracking = false
        })).ToList();

        long currentPriority = 1;
        foreach (var item in itemsToRenumber)
        {
            if (item.Priority != currentPriority)
            {
                item.Priority = currentPriority;
                repository.Update(item);
            }

            currentPriority++;
        }
    }
}
