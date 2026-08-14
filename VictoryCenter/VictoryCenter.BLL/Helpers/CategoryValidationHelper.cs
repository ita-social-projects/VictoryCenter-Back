using FluentResults;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Helpers;

public static class CategoryValidationHelper
{
    public static async Task<Result<ICollection<TCategory>>> ValidateAndGetCategoriesAsync<TCategory>(
        IRepositoryBase<TCategory> categoryRepository,
        IEnumerable<long> requestedCategoryIds)
        where TCategory : BaseEntity
    {
        var requestedIdsList = requestedCategoryIds.ToList();

        if (requestedIdsList.Count == 0)
        {
            return Result.Ok<ICollection<TCategory>>([]);
        }

        var retrievedCategoriesList = (await categoryRepository.GetAllAsync(new QueryOptions<TCategory>
        {
            Filter = category => requestedIdsList.Contains(category.Id),
            AsNoTracking = false
        })).ToList();

        if (retrievedCategoriesList.Count != requestedIdsList.Count)
        {
            var existingIds = retrievedCategoriesList.Select(c => c.Id).ToList();
            var missingIds = requestedIdsList.Except(existingIds).ToList();

            return Result.Fail<ICollection<TCategory>>(
                ErrorMessagesConstants.NotFound(missingIds, typeof(TCategory)));
        }

        return Result.Ok<ICollection<TCategory>>(retrievedCategoriesList);
    }
}
