using FluentValidation;
using FluentValidation.Results;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Helpers;

public static class ProgramSectionContentLocalizationValidationHelper
{
    public static void ValidateSections(
        IReadOnlyCollection<CreateHippotherapyProgramSectionLocalizationDto> sections,
        IReadOnlyDictionary<long, ContentType> contentTypesById)
    {
        if (sections.Count == 0)
        {
            return;
        }

        var sectionContents = sections
            .SelectMany(section => section.Contents ?? Enumerable.Empty<CreateHippotherapyProgramSectionContentLocalizationDto>())
            .ToList();

        var validContents = new HashSet<ContentType>
        {
            ContentType.Title,
            ContentType.Description,
            ContentType.Author,
            ContentType.FaqQuestion
        };

        var filteredContentTypes = contentTypesById
            .Where(c => validContents.Contains(c.Value))
            .ToDictionary(c => c.Key, c => c.Value);

        if (sectionContents.Count != filteredContentTypes.Count)
        {
            throw new ValidationException(new List<ValidationFailure>
            {
                new(nameof(sections),
                    $"Number of section contents ({sectionContents.Count}) does not match expected program contents ({contentTypesById.Count})")
            });
        }

        var failures = new List<ValidationFailure>();

        foreach (var section in sections)
        {
            if (section.Contents is null)
            {
                continue;
            }

            foreach (var content in section.Contents)
            {
                if (!filteredContentTypes.TryGetValue(content.EntityId, out var contentType))
                {
                    failures.Add(new ValidationFailure(
                        nameof(content.EntityId),
                        ErrorMessagesConstants.NotFound(content.EntityId, typeof(ProgramSectionContent))));
                    continue;
                }

                ValidateContentLocalizationByType(content, contentType, failures);
            }
        }

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }
    }

    private static void ValidateContentLocalizationByType(
        UpdateHippotherapyProgramSectionContentLocalizationDto content,
        ContentType contentType,
        List<ValidationFailure> failures)
    {
        var hasTitle = HasValue(content.Title);
        var hasDescription = HasValue(content.Description);
        var hasAuthor = HasValue(content.Author);
        var hasQuestion = HasValue(content.Question);
        var hasAnswer = HasValue(content.Answer);

        switch (contentType)
        {
            case ContentType.Title:
                RequireField(failures, nameof(content.Title), hasTitle);
                ForbidField(failures, nameof(content.Description), hasDescription, contentType);
                ForbidField(failures, nameof(content.Author), hasAuthor, contentType);
                ForbidField(failures, nameof(content.Question), hasQuestion, contentType);
                ForbidField(failures, nameof(content.Answer), hasAnswer, contentType);
                break;
            case ContentType.Description:
                RequireField(failures, nameof(content.Description), hasDescription);
                ForbidField(failures, nameof(content.Title), hasTitle, contentType);
                ForbidField(failures, nameof(content.Author), hasAuthor, contentType);
                ForbidField(failures, nameof(content.Question), hasQuestion, contentType);
                ForbidField(failures, nameof(content.Answer), hasAnswer, contentType);
                break;
            case ContentType.Author:
                RequireField(failures, nameof(content.Author), hasAuthor);
                ForbidField(failures, nameof(content.Title), hasTitle, contentType);
                ForbidField(failures, nameof(content.Description), hasDescription, contentType);
                ForbidField(failures, nameof(content.Question), hasQuestion, contentType);
                ForbidField(failures, nameof(content.Answer), hasAnswer, contentType);
                break;
            case ContentType.FaqQuestion:
                RequireField(failures, nameof(content.Question), hasQuestion);
                RequireField(failures, nameof(content.Answer), hasAnswer);
                ForbidField(failures, nameof(content.Title), hasTitle, contentType);
                ForbidField(failures, nameof(content.Description), hasDescription, contentType);
                ForbidField(failures, nameof(content.Author), hasAuthor, contentType);
                break;
            case ContentType.Image:
                ForbidField(failures, nameof(content.Title), hasTitle, contentType);
                ForbidField(failures, nameof(content.Description), hasDescription, contentType);
                ForbidField(failures, nameof(content.Author), hasAuthor, contentType);
                ForbidField(failures, nameof(content.Question), hasQuestion, contentType);
                ForbidField(failures, nameof(content.Answer), hasAnswer, contentType);
                break;
            default:
                failures.Add(new ValidationFailure(
                    nameof(contentType),
                    ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(contentType))));
                break;
        }
    }

    private static void RequireField(List<ValidationFailure> failures, string fieldName, bool hasValue)
    {
        if (!hasValue)
        {
            failures.Add(new ValidationFailure(fieldName, ErrorMessagesConstants.PropertyIsRequired(fieldName)));
        }
    }

    private static void ForbidField(List<ValidationFailure> failures, string fieldName, bool hasValue, ContentType contentType)
    {
        if (hasValue)
        {
            failures.Add(new ValidationFailure(fieldName, ErrorMessagesConstants.PropertyNotAllowedForContentType(fieldName, contentType)));
        }
    }

    private static bool HasValue(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
}
