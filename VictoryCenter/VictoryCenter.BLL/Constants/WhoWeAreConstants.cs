using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Constants;

public static class WhoWeAreConstants
{
    public static readonly Dictionary<SectionType, (int MinLen, int MaxLen)> ValidationDescriptionRules = new()
    {
        { SectionType.Main, (10, 300) },
        { SectionType.WhatWeDo, (10, 300) },
        { SectionType.WhoWeSupport, (10, 300) },
        { SectionType.Team, (10, 360) },
        { SectionType.People, (10, 60) }
    };
    public static readonly (int MinLen, int MaxLen) ValidationTitleRules = new(10, 50);
    public static string ContentCanNotBeNull => "Content cannot be null";

    public static string DtoHasWrongContentType(long dtoId, ContentType expected, ContentType received)
    {
        return $"Dto with id {dtoId} has wrong content type. Expected: {expected}, Received: {received}";
    }

    public static string EntityDoesNotBelongToTheSection(Type entity, long sectionId)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        return $"Entity {entity.Name} does not belong to the section with id {sectionId}";
    }

    public static string EntityIsNotRightContent(Type content)
    {
        ArgumentNullException.ThrowIfNull(content, nameof(content));

        return $"Entity is not {content.Name}";
    }

    public static string NoValidationRulesDefinedForSectionType(SectionType sectionType)
    {
        return $"No validation rules defined for section type: {sectionType}";
    }
}
