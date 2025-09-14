using System.Linq.Expressions;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Services.ReorderService;

/*public interface IOrderableEntity<TKey>
    where TKey : struct
{
    TKey Id { get; set; }
    TKey? NextElementId { get; set; }
}*/

public interface IReorderService
{
    Task<Result> MoveElement<TEntity, TKey>(
        TKey elementId,
        TKey? afterElementId,
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity<TKey>
        where TKey : struct;

    Task<TEntity?> GetLastElement<TEntity, TKey>(
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity<TKey>
        where TKey : struct;

    Task<Result<PaginationResult<TEntity>>> GetOrderedPageAsync<TEntity, TKey>(
        int offset,
        int limit,
        Expression<Func<TEntity, bool>>? groupSelector = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        where TEntity : class, IOrderableEntity<TKey>
        where TKey : struct;

    Task<Result> RemoveFromListAsync<TEntity, TKey>(TKey elementId, Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity<TKey>
        where TKey : struct;

    Task<Result> AppendToGroupEndAsync<TEntity, TKey>(TEntity elementToAppend, Expression<Func<TEntity, bool>> groupSelector)
        where TEntity : class, IOrderableEntity<TKey>
        where TKey : struct;
}

public class ReorderService : IReorderService
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    public ReorderService(IRepositoryWrapper repositoryWrapper)
    {
        _repositoryWrapper = repositoryWrapper;
    }

    public async Task<TEntity?> GetLastElement<TEntity, TKey>(Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity<TKey>
        where TKey : struct
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

    public async Task<Result> MoveElement<TEntity, TKey>(
        TKey elementId,
        TKey? afterElementId,
        Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        if (Equals(elementId, afterElementId))
        {
            return Result.Fail("Cannot move an element after itself.");
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

            var elementToMove = allElementsInGroup.FirstOrDefault(e => e.Id.Equals(elementId));
            if (elementToMove == null)
            {
                return Result.Fail($"Element with ID {elementId} not found in the specified group.");
            }

            var currentPrevious = allElementsInGroup.FirstOrDefault(e => Equals(e.NextElementId, elementId));

            if (currentPrevious != null)
            {
                currentPrevious.NextElementId = elementToMove.NextElementId;
                repository.Update(currentPrevious);
            }

            if (afterElementId == null)
            {
                var currentHead = allElementsInGroup.FirstOrDefault(e =>
                    !e.Id.Equals(elementId) &&
                    !allElementsInGroup.Any(p => Equals(p.NextElementId, e.Id)));

                elementToMove.NextElementId = currentHead?.Id;
            }
            else
            {
                var newPrevious = allElementsInGroup.FirstOrDefault(e => e.Id.Equals(afterElementId.Value));
                if (newPrevious == null)
                {
                    return Result.Fail($"Target element with ID {afterElementId} not found to move after.");
                }

                elementToMove.NextElementId = newPrevious.NextElementId;
                newPrevious.NextElementId = elementToMove.Id;
                repository.Update(newPrevious);
            }

            repository.Update(elementToMove);

            await _repositoryWrapper.SaveChangesAsync();
            scope.Complete();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"An error occurred during reordering: {ex.Message}");
        }
    }

    public async Task<Result<PaginationResult<TEntity>>> GetOrderedPageAsync<TEntity, TKey>(
        int offset,
        int limit,
        Expression<Func<TEntity, bool>>? groupSelector = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        where TEntity : class, IOrderableEntity<TKey>
        where TKey : struct
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
            return Result.Ok(new PaginationResult<TEntity>([], 0));
        }

        var sortedList = new List<TEntity>(allItemsInGroup.Count);
        var itemsById = allItemsInGroup.ToDictionary(item => item.Id);

        var allNextIds = new HashSet<TKey>(allItemsInGroup
            .Where(i => i.NextElementId.HasValue)
            .Select(i => i.NextElementId!.Value));

        TEntity? current = allItemsInGroup.FirstOrDefault(i => !allNextIds.Contains(i.Id));

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

        return Result.Ok(new PaginationResult<TEntity>(paginatedItems, totalCount));
    }

    public async Task<Result> RemoveFromListAsync<TEntity, TKey>(TKey elementId, Expression<Func<TEntity, bool>>? groupSelector = null)
        where TEntity : class, IOrderableEntity<TKey>
        where TKey : struct
    {
        var repository = _repositoryWrapper.GetRepository<TEntity>();

        var allItems = (await repository.GetAllAsync(new QueryOptions<TEntity> { Filter = groupSelector, AsNoTracking = false })).ToList();

        var elementToRemove = allItems.FirstOrDefault(e => e.Id.Equals(elementId));
        if (elementToRemove == null)
        {
            return Result.Ok();
        }

        var previousElement = allItems.FirstOrDefault(e => object.Equals(e.NextElementId, elementToRemove.Id));

        if (previousElement != null)
        {
            previousElement.NextElementId = elementToRemove.NextElementId;
            repository.Update(previousElement);
        }

        return Result.Ok();
    }

    public async Task<Result> AppendToGroupEndAsync<TEntity, TKey>(TEntity elementToAppend, Expression<Func<TEntity, bool>> groupSelector)
        where TEntity : class, IOrderableEntity<TKey>
        where TKey : struct
    {
        var repository = _repositoryWrapper.GetRepository<TEntity>();

        var allItemsInNewGroup = (await repository.GetAllAsync(new QueryOptions<TEntity> {
            Filter = groupSelector,
            AsNoTracking = false
        })).ToList();

        var lastElement = allItemsInNewGroup.FirstOrDefault(e => e.NextElementId == null);

        if (lastElement != null)
        {
            // Якщо в групі вже є елементи, ставимо наш елемент після останнього
            lastElement.NextElementId = elementToAppend.Id;
            repository.Update(lastElement);
        }

        elementToAppend.NextElementId = null;
        repository.Update(elementToAppend);

        return Result.Ok();
    }
}
