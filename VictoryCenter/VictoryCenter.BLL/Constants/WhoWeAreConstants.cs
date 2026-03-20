using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Constants;

public static class WhoWeAreConstants
{
    /// <summary>
    /// Validation rules for title content in the "Who We Are" section.
    /// </summary>
    public static readonly (int MinLen, int MaxLen) ValidationTitleRules = new(10, CalculateHighestCharactersLimitForRichInput(50));

    /// <summary>
    /// Validation rules for description content grouped by section type.
    /// </summary>
    public static readonly IReadOnlyDictionary<SectionType, (int MinLen, int MaxLen)> ValidationDescriptionRules = new Dictionary<SectionType, (int MinLen, int MaxLen)>
    {
        { SectionType.Main, (10, CalculateHighestCharactersLimitForRichInput(300)) },
        { SectionType.WhatWeDo, (10, CalculateHighestCharactersLimitForRichInput(300)) },
        { SectionType.WhoWeSupport, (10, CalculateHighestCharactersLimitForRichInput(300)) },
        { SectionType.Team, (10, CalculateHighestCharactersLimitForRichInput(400)) },
        { SectionType.People, (10, CalculateHighestCharactersLimitForRichInput(60)) }
    };

    public static string DtoHasWrongContentType(long dtoId, ContentType expected, ContentType received)
    {
        return $"Dto with id {dtoId} has wrong content type. Expected: {expected}, Received: {received}";
    }

    public static string EntityDoesNotBelongToTheSection(Type entity, SectionType sectionType)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        return $"Entity {entity.Name} does not belong to the {sectionType} section";
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

    /// <summary>
    /// Calculates the maximum allowed rich-text payload length for a given raw text limit.
    /// </summary>
    /// <param name="rawLimit">The raw character limit for plain user text.</param>
    /// <returns>
    /// The upper bound for serialized rich-text content, accounting for formatting tags.
    /// Formula: <c>(rawLimit * 25) + 7</c>, where:
    /// <list type="bullet">
    /// <item><description><c>25</c> = 1 user character + up to 24 formatting characters (for example, <c>&lt;i&gt;&lt;strong&gt;a&lt;/strong&gt;&lt;/i&gt;</c>).</description></item>
    /// <item><description><c>7</c> = paragraph wrapper <c>&lt;p&gt;&lt;/p&gt;</c>.</description></item>
    /// </list>
    /// Example: for <c>rawLimit = 1</c>, payload <c>&lt;p&gt;&lt;i&gt;&lt;strong&gt;a&lt;/strong&gt;&lt;/i&gt;&lt;/p&gt;</c> has length 32, which equals <c>(1 * 25) + 7</c>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="rawLimit"/> is negative.</exception>
    public static int CalculateHighestCharactersLimitForRichInput(int rawLimit)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(rawLimit);

        return checked((rawLimit * 25) + 7);
    }
}
