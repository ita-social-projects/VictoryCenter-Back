using FluentValidation;
using FluentValidation.Results;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Create;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Helpers;

public class HistorySectionContentLocalizationValidationHelper
{
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

            ValidateContentLocalizationByType(content, contentType);
        }
    }

    private static void ValidateContentLocalizationByType(
        IHistoryContentLocalization content,
        ContentType contentType)
    {
        var hasTitle = HasValue(content.Title);
        var hasDescription = HasValue(content.Description);

        switch (contentType)
        {
            case ContentType.Title:
                RequireField(nameof(content.Title), hasTitle);
                ForbidField(nameof(content.Description), hasDescription, contentType);
                break;
            case ContentType.Description:
                RequireField(nameof(content.Description), hasDescription);
                ForbidField(nameof(content.Title), hasTitle, contentType);
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

    private static void RequireField(string fieldName, bool hasValue)
    {
        if (!hasValue)
        {
            throw new ValidationException(
            [
                new ValidationFailure(fieldName, ErrorMessagesConstants.PropertyIsRequired(fieldName))
            ]);
        }
    }

    private static void ForbidField(string fieldName, bool hasValue, ContentType contentType)
    {
        if (hasValue)
        {
            throw new ValidationException(
            [
                new ValidationFailure(fieldName, ErrorMessagesConstants.PropertyNotAllowedForContentType(fieldName, contentType))
            ]);
        }
    }

    private static bool HasValue(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
}
