namespace VictoryCenter.BLL.Constants;

public static class MainPageConstants
{
    /// <summary>
    /// Validation rules for common title content in main page sections.
    /// </summary>
    public static readonly (int MinLen, int MaxLen) ValidationTitleRules = new(10, CalculateHighestCharactersLimitForRichInput(50));

    /// <summary>
    /// Validation rules for common description content in main page sections.
    /// </summary>
    public static readonly (int MinLen, int MaxLen) ValidationDescriptionRules = new(10, CalculateHighestCharactersLimitForRichInput(300));

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

    public static class Localization
    {
        /// <summary>
        /// Validation rules for localized title content in main page sections.
        /// </summary>
        public static readonly (int MinLen, int MaxLen) ValidationTitleRules = new(10, CalculateHighestCharactersLimitForRichInput(50));

        /// <summary>
        /// Validation rules for localized title block description content.
        /// </summary>
        public static readonly (int MinLen, int MaxLen) ValidationTitleBlockDescriptionRules = new(10, CalculateHighestCharactersLimitForRichInput(300));

        /// <summary>
        /// Validation rules for localized section description content.
        /// </summary>
        public static readonly (int MinLen, int MaxLen) ValidationSectionDescriptionRules = new(10, CalculateHighestCharactersLimitForRichInput(1000));
    }

    public static class ImpactStatistic
    {
        /// <summary>
        /// Validation rules for impact statistic title content.
        /// </summary>
        public static readonly (int MinLen, int MaxLen) ValidationTitleRules = new(5, CalculateHighestCharactersLimitForRichInput(100));

        public static int ExactMetricCount => 4;
    }

    public static class Metric
    {
        /// <summary>
        /// Validation rules for metric name content.
        /// </summary>
        public static readonly (int MinLen, int MaxLen) ValidationNameRules = new(2, CalculateHighestCharactersLimitForRichInput(20));
    }
}
