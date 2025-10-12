using System.Linq.Expressions;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Exceptions.ReorderExceptions;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Services.ReorderService;

/// <summary>
/// Service responsible for reordering entities that implement <see cref="IOrderableEntity"/>.
/// </summary>
public class ReorderService : IReorderService
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public ReorderService(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    /// <summary>
    /// Reorders entities by swapping their priorities according to the specified <paramref name="idsOrder"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity being reordered. Must implement <see cref="IOrderableEntity"/>.</typeparam>
    /// <param name="idsOrder">
    /// A list of entity ids representing the desired order. The list may contain duplicates (they will be deduplicated).
    /// If null or empty the method returns immediately without changes.
    /// </param>
    /// <param name="idSelector">Expression selecting the entity id from <typeparamref name="TEntity"/> (e.g. <c>e => e.Id</c>).</param>
    /// <param name="groupSelector">Optional filter expression to restrict the set of entities that participate in reorder (e.g. entities of the same parent).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ReorderException">
    /// Thrown when distinct ids exceed the maximum allowed, element priorities is not consequetive or
    /// when some ids were not found in repository.
    /// </exception>
    /// <exception cref="ArgumentNullException">If <paramref name="idSelector"/> is null.</exception>
    /// <exception cref="ArgumentException">If <paramref name="idSelector"/> returns two or more elements with same id.</exception>
    /// <remarks>
    /// The method performs the update in two steps: it assigns temporary negative priorities to avoid unique-constraint collisions,
    /// then assigns new priorities from the original ordering. It calls <c>SaveChangesAsync</c> twice to persist intermediate and final states.
    /// </remarks>
    public async Task SwapElementsAsync<TEntity>(
        List<long> idsOrder,
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity
    {
        if (idsOrder == null || idsOrder.Count <= 1)
        {
            return;
        }

        idsOrder = idsOrder.Distinct().ToList();

        if (idsOrder.Count > ReorderConstants.MaxElementsSwapCount)
        {
            throw new ReorderException(ReorderConstants.ExceededMaxElementsSwapCount(idsOrder.Count));
        }

        var repository = _repositoryWrapper.GetRepository<TEntity>();
        var idSelectorCompiled = idSelector.Compile();

        var entities = (await repository.GetAllAsync(new QueryOptions<TEntity>
        {
            Filter = groupSelector,
            AsNoTracking = false
        })).Where(e => idsOrder.Contains(idSelectorCompiled(e))).ToList();

        if (entities.Count != idsOrder.Count)
        {
            throw new ReorderException(ReorderConstants.NotAllEntitiesFoundForReorder(foundCount: entities.Count, expectedCount: idsOrder.Count));
        }

        var idToEntityMap = entities.ToDictionary(idSelectorCompiled);
        var oldPriorities = entities.OrderBy(e => e.Priority).Select(e => e.Priority).ToList();

        // Check sequential order of old priorities
        for (int i = 1; i < oldPriorities.Count; i++)
        {
            if (oldPriorities[i] != oldPriorities[i - 1] + 1)
            {
                throw new ReorderException(ReorderConstants.ElementsPrioritiesIsNotInSequentialOrder);
            }
        }

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

    /// <summary>
    /// Calculates the next display order (<see cref="IOrderableEntity.Priority"/>) for a new entity.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity. Must implement <see cref="IOrderableEntity"/>.</typeparam>
    /// <param name="groupSelector">Optional filter expression narrowing the scope for which the next priority is calculated (e.g. same parent group).</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> whose result is the next free priority (maximum existing priority + 1).
    /// If there are no entities, returns 1.
    /// </returns>
    /// <exception cref="InvalidOperationException">If repository query fails or the underlying repository throws an exception.</exception>
    public async Task<long> GetNextDisplayOrderAsync<TEntity>(
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity
    {
        var repository = _repositoryWrapper.GetRepository<TEntity>();
        var maxPriority = await repository.MaxAsync(e => e.Priority, groupSelector);
        return (maxPriority ?? 0) + 1;
    }

    /// <summary>
    /// Renumbers priorities for all entities in ascending order starting from 1.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity. Must implement <see cref="IOrderableEntity"/>.</typeparam>
    /// <param name="groupSelector">Optional filter expression to limit which entities to renumber (e.g. entities in the same group).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task RenumberPriorityAsync<TEntity>(
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity
    {
        var repository = _repositoryWrapper.GetRepository<TEntity>();
        var itemsToRenumber = await repository.GetAllAsync(new QueryOptions<TEntity>
        {
            Filter = groupSelector,
            OrderByASC = e => e.Priority,
            AsNoTracking = false
        });

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

        await _repositoryWrapper.SaveChangesAsync();
    }
}
