using FluentResults;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Helpers;

public static class CategoryValidationHelper
{
    public static async Task<Result<ICollection<HippotherapyProgramCategory>>> ValidateAndGetCategoriesAsync(
        IRepositoryWrapper repositoryWrapper,
        IEnumerable<long> requestedCategoryIds)
    {
        var requestedIdsList = requestedCategoryIds.ToList();

        var retrievedCategoriesList = (await repositoryWrapper
            .HippotherapyProgramCategoriesRepository.GetAllAsync(new QueryOptions<HippotherapyProgramCategory>
            {
                Filter = category => requestedIdsList.Contains(category.Id),
                AsNoTracking = false
            })).ToList();

        if (retrievedCategoriesList.Count != requestedIdsList.Count)
        {
            var existingIds = retrievedCategoriesList.Select(c => c.Id).ToList();
            var missingIds = requestedIdsList.Except(existingIds).ToList();

            return Result.Fail<ICollection<HippotherapyProgramCategory>>(
                ErrorMessagesConstants.NotFound(string.Join(", ", missingIds), typeof(HippotherapyProgramCategory)));
        }

        return Result.Ok<ICollection<HippotherapyProgramCategory>>(retrievedCategoriesList);
    }
}
