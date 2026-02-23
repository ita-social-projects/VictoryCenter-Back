using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Constants.Localization;

public static class WhoWeAreContentLocalizationConstants
{
    public static readonly (int MinLen, int MaxLen) ValidationTitleRules = new(10, 50);
    public static readonly IReadOnlyDictionary<SectionType, (int MinLen, int MaxLen)> ValidationDescriptionRules = new Dictionary<SectionType, (int MinLen, int MaxLen)>
    {
        { SectionType.Main, (10, 600) },
        { SectionType.WhatWeDo, (10, 600) },
        { SectionType.WhoWeSupport, (10, 600) },
        { SectionType.Team, (10, 800) },
        { SectionType.People, (10, 200) }
    };

    public static string CannotCreateLocalizationForContentType(Type contentType, long entityId)
    {
        ArgumentNullException.ThrowIfNull(contentType);

        return $"Cannot create localization for {contentType.Name} (EntityId: {entityId}) - no localizable fields.";
    }

    public static string FieldIsRequiredForContentType(string fieldName, Type contentType, long entityId)
    {
        ArgumentNullException.ThrowIfNull(contentType);

        return $"{fieldName} is required for {contentType.Name} (EntityId: {entityId}).";
    }

    public static string NoValidationRulesDefinedForSectionType(SectionType sectionType)
    {
        return $"No validation rules defined for section type: {sectionType}";
    }
}
