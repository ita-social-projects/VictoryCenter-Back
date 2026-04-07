using VictoryCenter.BLL.DTOs.Admin.HistorySection;
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

    public static string GetGroupCompositionErrorMessage(CreateHistorySectionDto section)
        => $"Template {section.Template} has invalid group composition";

    public static string GetTitlesCountErrorMessage(CreateHistorySectionDto section)
        => GetCountErrorMessage(section, ContentType.Title, GetRequirements(section.Template).TitleCount, "title(s)");

    public static string GetDescriptionsCountErrorMessage(CreateHistorySectionDto section)
        => GetCountErrorMessage(section, ContentType.Description, GetRequirements(section.Template).DescriptionCount, "description(s)");

    public static string GetImagesCountErrorMessage(CreateHistorySectionDto section)
        => GetCountErrorMessage(section, ContentType.Image, GetRequirements(section.Template).ImageCount, "image(s)");

    public static string GetTitleLengthErrorMessage(CreateHistorySectionDto section)
        => GetLengthErrorMessage(GetRequirements(section.Template).TitleLength, "title");

    public static string GetDescriptionLengthErrorMessage(CreateHistorySectionDto section)
        => GetLengthErrorMessage(GetRequirements(section.Template).DescriptionLength, "description");

    private static int CountByType(CreateHistorySectionDto section, ContentType type)
        => (section.Contents ?? []).Count(c => c.ContentType == type);

    private static string GetCountErrorMessage(
        CreateHistorySectionDto section,
        ContentType type,
        (int Min, int Max) req,
        string label)
    {
        var actual = CountByType(section, type);
        return $"Template {section.Template} requires between {req.Min} and {req.Max} {label}, but received {actual}";
    }

    private static string GetLengthErrorMessage((int Min, int Max) req, string label)
        => $"Each {label} must be between {req.Min} and {req.Max} characters";
}
