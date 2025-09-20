using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.BLL.Interfaces.ReorderService;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Services.ReorderService;

public class ReorderException : Exception
{
    public ReorderException(string message)
        : base(message)
    {
    }
}

public class ReorderService : IReorderService
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public ReorderService(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<TEntity?> GetLastElement<TEntity>(
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity
    {
        var repository = _repositoryWrapper.GetRepository<TEntity>();

        var fetchedElements = await repository.GetAllAsync(new QueryOptions<TEntity>
        {
            AsNoTracking = true,
            Filter = e => e.NextElementId == null,
        });

        if (!fetchedElements.Any())
        {
            return default;
        }

        var elements = groupSelector == null
            ? fetchedElements.ToList()
            : fetchedElements.Where(groupSelector.Compile()).ToList();

        if (elements.Count > 1)
        {
            throw new InvalidOperationException("Multiple last elements found.");
        }

        return elements.SingleOrDefault();
    }

    public async Task MoveElement<TEntity>(
        long elementId,
        long? prevElementId,
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity
    {
        if (elementId == prevElementId)
        {
            throw new ReorderException("Cannot move an element after itself.");
        }

        var repository = _repositoryWrapper.GetRepository<TEntity>();

        using var scope = _repositoryWrapper.BeginTransaction();

        try
        {
            var allElementsInGroup = (await repository.GetAllAsync(new QueryOptions<TEntity>
            {
                AsNoTracking = true,
                Filter = groupSelector
            })).ToList();

            var idSelectorCompiled = idSelector.Compile();

            var elementToMove = allElementsInGroup.FirstOrDefault(e => idSelectorCompiled(e) == elementId);
            if (elementToMove == null)
            {
                throw new ReorderException($"Element with ID {elementId} not found in the specified group.");
            }

            var currentPrevious = allElementsInGroup.FirstOrDefault(e => e.NextElementId == elementId);

            if (currentPrevious != null)
            {
                currentPrevious.NextElementId = elementToMove.NextElementId;
                repository.Update(currentPrevious);
            }

            if (prevElementId == null)
            {
                var currentHead = allElementsInGroup.FirstOrDefault(e =>
                    idSelectorCompiled(e) != elementId &&
                    !allElementsInGroup.Any(p => p.NextElementId == idSelectorCompiled(e)));

                elementToMove.NextElementId = currentHead != null ? idSelectorCompiled(currentHead) : null;
            }
            else
            {
                var newPrevious = allElementsInGroup.FirstOrDefault(e => idSelectorCompiled(e) == prevElementId.Value);
                if (newPrevious == null)
                {
                    throw new ReorderException($"Target element with ID {prevElementId} not found to move after.");
                }

                elementToMove.NextElementId = newPrevious.NextElementId;
                newPrevious.NextElementId = elementId;
                repository.Update(newPrevious);
            }

            repository.Update(elementToMove);

            await _repositoryWrapper.SaveChangesAsync();
            scope.Complete();
        }
        catch (Exception ex)
        {
            throw new ReorderException($"An error occurred during reordering: {ex.Message}");
        }
    }

    public async Task<PaginationResult<TEntity>> GetOrderedPageAsync<TEntity>(
        int offset,
        int limit,
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>>? groupSelector = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        where TEntity : class, IOrderableEntity
    {
        var repository = _repositoryWrapper.GetRepository<TEntity>();

        var queryOptions = new QueryOptions<TEntity>
        {
            Filter = groupSelector,
            Include = include,
            AsNoTracking = true
        };
        var allItemsInGroup = (await repository.GetAllAsync(queryOptions)).ToList();

        if (allItemsInGroup.Count == 0)
        {
            return new PaginationResult<TEntity>([], 0);
        }

        var sortedList = new List<TEntity>(allItemsInGroup.Count);
        var idSelectorCompiled = idSelector.Compile();
        var itemsById = allItemsInGroup.ToDictionary(item => idSelectorCompiled(item));

        var allNextIds = new HashSet<long>(allItemsInGroup
            .Where(i => i.NextElementId.HasValue)
            .Select(i => i.NextElementId!.Value));

        TEntity? current = allItemsInGroup.FirstOrDefault(i => !allNextIds.Contains(idSelectorCompiled(i)));

        // Якщо голова не знайдена (можливо, є цикл), повертаємо помилку або намагаємось відновити
        if (current == null && allItemsInGroup.Any())
        {
            // Це може статись, якщо є циклічне посилання. Беремо перший для відновлення.
            current = allItemsInGroup.First();
        }

        while (current != null && sortedList.Count < allItemsInGroup.Count)
        {
            sortedList.Add(current);
            if (current.NextElementId.HasValue && itemsById.TryGetValue(current.NextElementId.Value, out var nextItem))
            {
                current = nextItem;
            }
            else
            {
                current = null;
            }
        }

        var totalCount = sortedList.Count;
        var paginatedItems = sortedList.Skip(offset).Take(limit).ToArray();

        return new PaginationResult<TEntity>(paginatedItems, totalCount);
    }

    public async Task RemoveLinksFromListAsync<TEntity>(
        long elementId,
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity
    {
        var idSelectorCompiled = idSelector.Compile();
        var repository = _repositoryWrapper.GetRepository<TEntity>();

        var allItems = (await repository.GetAllAsync(new QueryOptions<TEntity>
        {
            Filter = groupSelector,
            AsNoTracking = false
        })).ToList();

        var elementToRemove = allItems.FirstOrDefault(e => idSelectorCompiled(e) == elementId);
        if (elementToRemove != null)
        {
            throw new ReorderException($"Element with ID {elementId} is presented in specified group. Remove it before unkink");
        }

        var previousElement = allItems.FirstOrDefault(e => e.NextElementId == elementId);

        if (previousElement != null)
        {
            previousElement.NextElementId = elementToRemove.NextElementId;
            repository.Update(previousElement);
            await _repositoryWrapper.SaveChangesAsync();
        }
    }

    public async Task AppendToGroupEndAsync<TEntity>(
        long elementId,
        Expression<Func<TEntity, long>> idSelector,
        Expression<Func<TEntity, bool>> groupSelector)
        where TEntity : class, IOrderableEntity
    {
        var repository = _repositoryWrapper.GetRepository<TEntity>();
        var idSelectorCompiled = idSelector.Compile();

        var allItemsInNewGroup = (await repository.GetAllAsync(new QueryOptions<TEntity>
        {
            Filter = groupSelector,
            AsNoTracking = false
        })).ToList();

        var targetElement = allItemsInNewGroup.FirstOrDefault(e => idSelectorCompiled(e) == elementId);
        if (targetElement == null)
        {
            throw new InvalidOperationException("Element to append does not belong to the specified group.");
        }

        if (targetElement.NextElementId != null)
        {
            throw new InvalidOperationException("Element to append is already in the list.");
        }

        var lastElement = allItemsInNewGroup.FirstOrDefault(e => e.NextElementId == null);

        if (lastElement != null)
        {
            lastElement.NextElementId = idSelectorCompiled(targetElement);
            repository.Update(lastElement);
        }

        targetElement.NextElementId = null;
        repository.Update(targetElement);
        await _repositoryWrapper.SaveChangesAsync();
    }
}
