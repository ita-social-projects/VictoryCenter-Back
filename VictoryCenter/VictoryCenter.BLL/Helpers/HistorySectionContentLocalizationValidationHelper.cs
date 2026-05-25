using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Common;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Helpers;

public class HistorySectionContentLocalizationValidationHelper
{
    public static Result ValidateSectionContents<TContent>(
        long sectionId,
        IEnumerable<TContent> requestContents,
        IEnumerable<HistorySectionContent> existingContents)
        where TContent : IHistoryContentLocalization
    {
        var requestContentIds = requestContents.Select(c => c.EntityId).ToHashSet();

        var missingContentIds = existingContents
            .Where(c => c.ContentType != ContentType.Image)
            .Select(c => c.Id)
            .Where(id => !requestContentIds.Contains(id))
            .ToList();

        if (missingContentIds.Count > 0)
        {
            return Result.Fail(ErrorMessagesConstants.MissingContentsLocalization(sectionId, missingContentIds));
        }

        var contentTypesById = existingContents.ToDictionary(c => c.Id, c => c.ContentType);

        ValidateHistoryContents(requestContents, contentTypesById);

        return Result.Ok();
    }

    public static void ValidateHistoryContents<TContent>(
        IEnumerable<TContent> localizationContents,
        IReadOnlyDictionary<long, ContentType> contentTypesById)
        where TContent : IHistoryContentLocalization
    {
        var allowedTypes = new HashSet<ContentType> { ContentType.Title, ContentType.Description };

        foreach (var content in localizationContents)
        {
            if (!contentTypesById.TryGetValue(content.EntityId, out var contentType))
            {
                throw new ValidationException(
                [
                    new ValidationFailure("EntityId", ErrorMessagesConstants.NotFound(content.EntityId, typeof(HistorySectionContent)))
                ]);
            }

            if (!allowedTypes.Contains(contentType))
            {
                throw new ValidationException(
                [
                    new ValidationFailure(
                        "ContentType",
                        $"Content {content.EntityId} has type {contentType}, which is not allowed for history localization. Only Title and Description are allowed.")
                ]);
            }

            ValidateContentLocalizationByTypes(content, contentType);
        }
    }

    private static void ValidateContentLocalizationByTypes(
        IHistoryContentLocalization content,
        ContentType contentType)
    {
        var hasTitle = ProgramSectionContentLocalizationValidationHelper.HasValue(content.Title);
        var hasDescription = ProgramSectionContentLocalizationValidationHelper.HasValue(content.Description);

        switch (contentType)
        {
            case ContentType.Title:
                ProgramSectionContentLocalizationValidationHelper.RequireField(nameof(content.Title), hasTitle);
                ProgramSectionContentLocalizationValidationHelper.ForbidField(nameof(content.Description), hasDescription, contentType);
                break;
            case ContentType.Description:
                ProgramSectionContentLocalizationValidationHelper.RequireField(nameof(content.Description), hasDescription);
                ProgramSectionContentLocalizationValidationHelper.ForbidField(nameof(content.Title), hasTitle, contentType);
                break;
            default:
                throw new ValidationException(
                [
                    new ValidationFailure(
                        nameof(contentType),
                        ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(contentType)))
                ]);
        }
    }
}
