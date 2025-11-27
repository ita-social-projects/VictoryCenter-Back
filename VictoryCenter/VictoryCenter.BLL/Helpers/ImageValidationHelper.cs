using FluentResults;
using VictoryCenter.BLL.Constants;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Helpers;

public static class ImageValidationHelper
{
    public static async Task<Result<Image?>> ValidateAndGetImageAsync(
        IRepositoryWrapper repositoryWrapper,
        long? imageId)
    {
        if (!imageId.HasValue)
        {
            return Result.Ok<Image?>(null);
        }

        var retrievedImage = await FetchImageFromRepositoryAsync(repositoryWrapper, imageId.Value);

        if (retrievedImage is null)
        {
            return BuildImageNotFoundError(imageId.Value);
        }

        return Result.Ok<Image?>(retrievedImage);
    }

    private static async Task<Image?> FetchImageFromRepositoryAsync(
        IRepositoryWrapper repositoryWrapper,
        long imageId)
    {
        return await repositoryWrapper.ImageRepository
            .GetFirstOrDefaultAsync(new QueryOptions<Image>
            {
                Filter = image => image.Id == imageId,
                AsNoTracking = false
            });
    }

    private static Result<Image?> BuildImageNotFoundError(long imageId)
    {
        var errorMessage = ErrorMessagesConstants.NotFound(
            imageId.ToString(),
            typeof(Image));

        return Result.Fail<Image?>(errorMessage);
    }
}
