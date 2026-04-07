using FluentResults;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;
using VictoryCenter.BLL.Exceptions.BlobStorageExceptions;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Helpers;

public static class ImageValidationHelper
{
    public static async Task<Result<Image?>> ValidateAndGetImageAsync(
        IRepositoryWrapper repositoryWrapper,
        long? imageId)
    {
        try
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
        catch (BlobStorageException)
        {
            return Result.Fail<Image?>(ErrorMessagesConstants.FailedToRetrieveImage());
        }
    }

    public static async Task<Result<IReadOnlyDictionary<long, Image>>> ValidateAndGetImagesByIdsAsync(
        IRepositoryWrapper repositoryWrapper,
        IEnumerable<long> imageIds)
    {
        try
        {
            var ids = imageIds
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (ids.Count == 0)
            {
                return Result.Ok<IReadOnlyDictionary<long, Image>>(new Dictionary<long, Image>());
            }

            var images = await repositoryWrapper.ImageRepository.GetAllAsync(new QueryOptions<Image>
            {
                Filter = i => ids.Contains(i.Id),
                AsNoTracking = false
            });

            var imagesById = images.ToDictionary(i => i.Id);
            var missingIds = ids.Where(id => !imagesById.ContainsKey(id)).ToList();

            return missingIds.Count != 0
                ? Result.Fail<IReadOnlyDictionary<long, Image>>(ErrorMessagesConstants.NotFound(missingIds, typeof(Image)))
                : Result.Ok<IReadOnlyDictionary<long, Image>>(imagesById);
        }
        catch (BlobStorageException)
        {
            return Result.Fail<IReadOnlyDictionary<long, Image>>(ErrorMessagesConstants.FailedToRetrieveImage());
        }
    }

    public static async Task<Result> ValidateAndAssignProgramImagesAsync(
        IRepositoryWrapper repositoryWrapper, HippotherapyProgram program)
    {
        var backgroundImageResult = await ValidateAndGetImageAsync(
            repositoryWrapper, program.BackgroundImageId);

        if (backgroundImageResult.IsFailed)
        {
            return Result.Fail(backgroundImageResult.Errors);
        }

        var previewImageResult = await ValidateAndGetImageAsync(
            repositoryWrapper, program.PreviewImageId);

        if (previewImageResult.IsFailed)
        {
            return Result.Fail(previewImageResult.Errors);
        }

        program.BackgroundImage = backgroundImageResult.Value;
        program.PreviewImage = previewImageResult.Value;

        return Result.Ok();
    }

    public static Task<Result<IReadOnlyDictionary<long, Image>>> ValidateAndGetSectionImagesAsync(
        IRepositoryWrapper repositoryWrapper, List<CreateHippotherapyProgramSectionDto>? sections)
    {
        var sectionImageIds = (sections ?? [])
            .SelectMany(s => s.Contents ?? [])
            .Where(c => c.ContentType == ContentType.Image && c.ImageId.HasValue)
            .Select(c => c.ImageId!.Value);

        return ValidateAndGetImagesByIdsAsync(repositoryWrapper, sectionImageIds);
    }

    public static Task<Result<IReadOnlyDictionary<long, Image>>> ValidateAndGetSectionImagesAsync(
        IRepositoryWrapper repositoryWrapper, List<CreateHistorySectionDto>? sections)
    {
        var sectionImageIds = (sections ?? [])
            .SelectMany(s => s.Contents ?? [])
            .Where(c => c.ContentType == ContentType.Image && c.ImageId.HasValue)
            .Select(c => c.ImageId!.Value);

        return ValidateAndGetImagesByIdsAsync(repositoryWrapper, sectionImageIds);
    }

    public static async Task<Result> ValidateAndAssignSectionContentImagesAsync(
        IRepositoryWrapper repositoryWrapper,
        IEnumerable<HippotherapyProgramSection> sections)
    {
        var imageIds = (sections ?? [])
            .SelectMany(s => s.Contents)
            .OfType<ImageProgramContent>()
            .Select(c => c.ImageId);

        var imagesByIdResult = await ValidateAndGetImagesByIdsAsync(repositoryWrapper, imageIds);

        if (imagesByIdResult.IsFailed)
        {
            return Result.Fail(imagesByIdResult.Errors);
        }

        var imagesById = imagesByIdResult.Value;

        foreach (var content in (sections ?? []).SelectMany(s => s.Contents).OfType<ImageProgramContent>())
        {
            content.Image = imagesById[content.ImageId];
        }

        return Result.Ok();
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
            imageId,
            typeof(Image));

        return Result.Fail<Image?>(errorMessage);
    }
}
