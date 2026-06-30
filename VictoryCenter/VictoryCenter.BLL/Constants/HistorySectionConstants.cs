using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Constants;

public static class HistorySectionConstants
{
    public sealed record TemplateRequirementsConfig(
        (int Min, int Max) TitleCount = default,
        (int Min, int Max) TitleLength = default,
        (int Min, int Max) DescriptionCount = default,
        (int Min, int Max) DescriptionLength = default,
        (int Min, int Max) ImageCount = default
    );

    private static readonly Dictionary<HistorySectionTemplate, TemplateRequirementsConfig> TemplateRequirements = new()
    {
        [HistorySectionTemplate.QuadImagesBottom] = new(
            TitleCount: (1, 1),
            TitleLength: (5, 60),
            DescriptionCount: (1, 1),
            DescriptionLength: (10, 600),
            ImageCount: (4, 4)
        ),
        [HistorySectionTemplate.DualImagesBottom] = new(
            TitleCount: (1, 1),
            TitleLength: (5, 60),
            DescriptionCount: (1, 1),
            DescriptionLength: (10, 600),
            ImageCount: (2, 2)
        ),
        [HistorySectionTemplate.TextOnly] = new(
            TitleCount: (1, 1),
            TitleLength: (5, 60),
            DescriptionCount: (1, 1),
            DescriptionLength: (10, 600),
            ImageCount: (0, 0)
        ),
        [HistorySectionTemplate.TripleImagesBottom] = new(
            TitleCount: (1, 1),
            TitleLength: (5, 60),
            DescriptionCount: (1, 1),
            DescriptionLength: (10, 600),
            ImageCount: (3, 3)
        ),
        [HistorySectionTemplate.SingleImageBottom] = new(
            TitleCount: (1, 1),
            TitleLength: (5, 60),
            DescriptionCount: (1, 1),
            DescriptionLength: (10, 600),
            ImageCount: (1, 1)
        ),
        [HistorySectionTemplate.SingleImageTop] = new(
            TitleCount: (1, 1),
            TitleLength: (5, 60),
            DescriptionCount: (1, 1),
            DescriptionLength: (10, 600),
            ImageCount: (1, 1)
        ),
        [HistorySectionTemplate.SingleImageRight] = new(
            TitleCount: (1, 1),
            TitleLength: (5, 60),
            DescriptionCount: (1, 1),
            DescriptionLength: (10, 600),
            ImageCount: (1, 1)
        ),
    };
    public static TemplateRequirementsConfig GetRequirements(HistorySectionTemplate template)
        => TemplateRequirements[template];

    public static string GetGroupCompositionErrorMessage(HistorySectionTemplate template)
        => $"Template {template} has invalid group composition";

    public static string GetTitlesCountErrorMessage(HistorySectionTemplate template, int actual)
        => GetCountErrorMessage(template, GetRequirements(template).TitleCount, "title(s)", actual);

    public static string GetDescriptionsCountErrorMessage(HistorySectionTemplate template, int actual)
        => GetCountErrorMessage(template, GetRequirements(template).DescriptionCount, "description(s)", actual);

    public static string GetImagesCountErrorMessage(HistorySectionTemplate template, int actual)
        => GetCountErrorMessage(template, GetRequirements(template).ImageCount, "image(s)", actual);

    public static string GetTitleLengthErrorMessage(HistorySectionTemplate template)
        => GetLengthErrorMessage(GetRequirements(template).TitleLength, "title");

    public static string GetDescriptionLengthErrorMessage(HistorySectionTemplate template)
        => GetLengthErrorMessage(GetRequirements(template).DescriptionLength, "description");

    private static string GetCountErrorMessage(
        HistorySectionTemplate template,
        (int Min, int Max) req,
        string label,
        int actual)
    {
        return $"Template {template} requires between {req.Min} and {req.Max} {label}, but received {actual}";
    }

    private static string GetLengthErrorMessage((int Min, int Max) req, string label)
        => $"Each {label} must be between {req.Min} and {req.Max} characters";
}
