using FluentValidation;
using FluentValidation.Results;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Common;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Enums;
using HippotherapyProgramEntity = VictoryCenter.DAL.Entities.HippotherapyProgram;

namespace VictoryCenter.BLL.Helpers;

public static class ProgramSectionContentLocalizationValidationHelper
{
    public static void ValidateSections<TSection, TContent>(
        IReadOnlyCollection<TSection> sections,
        IReadOnlyDictionary<long, ContentType> contentTypesById,
        Func<TContent, long> getEntityId,
        HippotherapyProgramEntity programEntity)
        where TSection : BaseHippotherapyProgramSectionLocalizationDto<TContent>
        where TContent : BaseHippotherapyProgramSectionContentLocalizationDto
    {
        ArgumentNullException.ThrowIfNull(getEntityId);
        ArgumentNullException.ThrowIfNull(programEntity);

        var requiredContentTypes = new HashSet<ContentType>
        {
            ContentType.Title,
            ContentType.Description,
            ContentType.Author,
            ContentType.FaqQuestion
        };

        var failures = new List<ValidationFailure>();

        var expectedSectionIds = programEntity.Sections
            .Select(s => s.Id)
            .ToHashSet();

        var requestSectionIds = sections
            .Select(s => s.EntityId)
            .ToHashSet();

        var missingSectionIds = expectedSectionIds.Except(requestSectionIds).ToList();
        if (missingSectionIds.Count > 0)
        {
            throw new ValidationException(
            [
                new ValidationFailure(
                    nameof(sections),
                    $"Missing sections in localization request: {string.Join(", ", missingSectionIds)}.")
            ]);
        }

        var extraSectionIds = requestSectionIds.Except(expectedSectionIds).ToList();
        if (extraSectionIds.Count > 0)
        {
            throw new ValidationException(
            [
                new ValidationFailure(
                    nameof(sections),
                    $"Unknown sections in localization request: {string.Join(", ", extraSectionIds)}.")
            ]);
        }

        var programSectionsById = programEntity.Sections
            .ToDictionary(s => s.Id);

        var contentSectionIdByContentId = programEntity.Sections
            .SelectMany(section => section.Contents.Select(content => new
            {
                ContentId = content.Id,
                SectionId = section.Id
            }))
            .ToDictionary(x => x.ContentId, x => x.SectionId);

        foreach (var section in sections)
        {
            if (!programSectionsById.TryGetValue(section.EntityId, out var programSection))
            {
                continue;
            }

            var expectedRequiredContents = programSection.Contents
                .Where(c => requiredContentTypes.Contains(c.ContentType))
                .ToList();

            var expectedRequiredIds = expectedRequiredContents
                .Select(c => c.Id)
                .ToHashSet();

            var expectedTypeByContentId = expectedRequiredContents
                .ToDictionary(c => c.Id, c => c.ContentType);

            var providedContents = section.Contents ?? [];
            var providedIds = providedContents
                .Select(getEntityId)
                .ToList();

            var duplicateIds = providedIds
                .GroupBy(id => id)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateIds.Count > 0)
            {
                throw new ValidationException(
                [
                    new ValidationFailure(
                        nameof(sections),
                        $"Section {section.EntityId} contains duplicate content ids: {string.Join(", ", duplicateIds)}.")
                ]);
            }

            var providedIdSet = providedIds.ToHashSet();

            var missingRequiredIds = expectedRequiredIds.Except(providedIdSet).ToList();
            if (missingRequiredIds.Count > 0)
            {
                throw new ValidationException(
                [
                    new ValidationFailure(
                        nameof(sections),
                        $"Section {section.EntityId} is missing required content ids: {string.Join(", ", missingRequiredIds)}.")
                ]);
            }

            if (providedIdSet.Count != expectedRequiredIds.Count)
            {
                throw new ValidationException(
                [
                    new ValidationFailure(
                        nameof(sections),
                        $"Section {section.EntityId} has {providedIdSet.Count} contents, expected {expectedRequiredIds.Count} required contents.")
                ]);
            }

            foreach (var content in providedContents)
            {
                var entityId = getEntityId(content);

                if (!contentTypesById.TryGetValue(entityId, out var contentType))
                {
                    throw new ValidationException(
                    [
                        new ValidationFailure(
                            "EntityId",
                            ErrorMessagesConstants.NotFound(entityId, typeof(ProgramSectionContent)))
                    ]);
                }

                if (!expectedTypeByContentId.TryGetValue(entityId, out var expectedType))
                {
                    if (contentType == ContentType.Image)
                    {
                        throw new ValidationException(
                        [
                            new ValidationFailure(
                                "EntityId",
                                $"Section {section.EntityId}: content {entityId} has type {ContentType.Image}, which is not required for localization.")
                        ]);
                    }
                }

                ValidateContentLocalizationByType(content, expectedType);
            }
        }
    }

    private static void ValidateContentLocalizationByType(
        BaseHippotherapyProgramSectionContentLocalizationDto content,
        ContentType contentType)
    {
        var hasTitle = HasValue(content.Title);
        var hasDescription = HasValue(content.Description);
        var hasAuthor = HasValue(content.Author);
        var hasQuestion = HasValue(content.Question);
        var hasAnswer = HasValue(content.Answer);

        switch (contentType)
        {
            case ContentType.Title:
                RequireField(nameof(content.Title), hasTitle);
                ForbidField(nameof(content.Description), hasDescription, contentType);
                ForbidField(nameof(content.Author), hasAuthor, contentType);
                ForbidField(nameof(content.Question), hasQuestion, contentType);
                ForbidField(nameof(content.Answer), hasAnswer, contentType);
                break;
            case ContentType.Description:
                RequireField(nameof(content.Description), hasDescription);
                ForbidField(nameof(content.Title), hasTitle, contentType);
                ForbidField(nameof(content.Author), hasAuthor, contentType);
                ForbidField(nameof(content.Question), hasQuestion, contentType);
                ForbidField(nameof(content.Answer), hasAnswer, contentType);
                break;
            case ContentType.Author:
                RequireField(nameof(content.Author), hasAuthor);
                ForbidField(nameof(content.Title), hasTitle, contentType);
                ForbidField(nameof(content.Description), hasDescription, contentType);
                ForbidField(nameof(content.Question), hasQuestion, contentType);
                ForbidField(nameof(content.Answer), hasAnswer, contentType);
                break;
            case ContentType.FaqQuestion:
                RequireField(nameof(content.Question), hasQuestion);
                RequireField(nameof(content.Answer), hasAnswer);
                ForbidField(nameof(content.Title), hasTitle, contentType);
                ForbidField(nameof(content.Description), hasDescription, contentType);
                ForbidField(nameof(content.Author), hasAuthor, contentType);
                break;
            case ContentType.Image:
                ForbidField(nameof(content.Title), hasTitle, contentType);
                ForbidField(nameof(content.Description), hasDescription, contentType);
                ForbidField(nameof(content.Author), hasAuthor, contentType);
                ForbidField(nameof(content.Question), hasQuestion, contentType);
                ForbidField(nameof(content.Answer), hasAnswer, contentType);
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
