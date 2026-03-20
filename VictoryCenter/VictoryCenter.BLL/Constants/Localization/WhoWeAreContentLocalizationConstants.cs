using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Constants.Localization;

public static class WhoWeAreContentLocalizationConstants
{
    /// <summary>
    /// Validation rules for localized title content.
    /// </summary>
    public static readonly (int MinLen, int MaxLen) ValidationTitleRules = WhoWeAreConstants.ValidationTitleRules;

    /// <summary>
    /// Validation rules for localized description content grouped by section type.
    /// </summary>
    public static readonly IReadOnlyDictionary<SectionType, (int MinLen, int MaxLen)> ValidationDescriptionRules = WhoWeAreConstants.ValidationDescriptionRules;

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
