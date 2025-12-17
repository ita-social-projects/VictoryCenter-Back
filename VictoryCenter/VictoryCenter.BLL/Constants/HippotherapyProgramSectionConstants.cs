using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Constants;

public static class HippotherapyProgramSectionConstants
{
    public sealed record TemplateRequirementsConfig(
        (int Min, int Max) TitleCount,
        (int Min, int Max) TitleLength,
        (int Min, int Max) DescriptionCount,
        (int Min, int Max) DescriptionLength,
        (int Min, int Max) ImageCount
    );

    private static readonly Dictionary<ProgramSectionTemplate, TemplateRequirementsConfig>
        TemplateRequirements = new()
        {
            [ProgramSectionTemplate.QuadImagesBottom] = new(
                TitleCount: (1, 1),
                TitleLength: (5, 60),
                DescriptionCount: (1, 1),
                DescriptionLength: (10, 600),
                ImageCount: (4, 4)
            ),
            [ProgramSectionTemplate.DualImagesBottom] = new(
                TitleCount: (1, 1),
                TitleLength: (5, 60),
                DescriptionCount: (1, 1),
                DescriptionLength: (10, 600),
                ImageCount: (2, 2)
            ),
            [ProgramSectionTemplate.TextOnly] = new(
                TitleCount: (1, 1),
                TitleLength: (5, 60),
                DescriptionCount: (1, 1),
                DescriptionLength: (10, 600),
                ImageCount: (0, 0)
            ),
            [ProgramSectionTemplate.TripleImagesBottom] = new(
                TitleCount: (1, 1),
                TitleLength: (5, 60),
                DescriptionCount: (1, 1),
                DescriptionLength: (10, 600),
                ImageCount: (3, 3)
            ),
            [ProgramSectionTemplate.SingleImageBottom] = new(
                TitleCount: (1, 1),
                TitleLength: (5, 60),
                DescriptionCount: (1, 1),
                DescriptionLength: (10, 600),
                ImageCount: (1, 1)
            ),
            [ProgramSectionTemplate.SingleImageTop] = new(
                TitleCount: (1, 1),
                TitleLength: (5, 60),
                DescriptionCount: (1, 1),
                DescriptionLength: (10, 600),
                ImageCount: (1, 1)
            ),
            [ProgramSectionTemplate.SingleImageRight] = new(
                TitleCount: (1, 1),
                TitleLength: (5, 60),
                DescriptionCount: (1, 1),
                DescriptionLength: (10, 600),
                ImageCount: (1, 1)
            )
        };

    public static TemplateRequirementsConfig GetRequirements(ProgramSectionTemplate template)
        => TemplateRequirements[template];

    public static string GetTitlesCountErrorMessage(CreateHippotherapyProgramSectionDto section)
    {
        var req = GetRequirements(section.Template);
        return $"Template {section.Template} requires exactly {req.TitleCount.Min} title(s), but received {section.Titles?.Count ?? 0}";
    }

    public static string GetDescriptionsCountErrorMessage(CreateHippotherapyProgramSectionDto section)
    {
        var req = GetRequirements(section.Template);
        return $"Template {section.Template} requires exactly {req.DescriptionCount.Min} description(s), but received {section.Descriptions?.Count ?? 0}";
    }

    public static string GetImagesCountErrorMessage(CreateHippotherapyProgramSectionDto section)
    {
        var req = GetRequirements(section.Template);
        return $"Template {section.Template} requires exactly {req.ImageCount.Min} image(s), but received {section.ImageIds?.Count ?? 0}";
    }

    public static string GetTitleLengthErrorMessage(CreateHippotherapyProgramSectionDto section)
    {
        var req = GetRequirements(section.Template);
        return $"Each title must be between {req.TitleLength.Min} and {req.TitleLength.Max} characters";
    }

    public static string GetDescriptionLengthErrorMessage(CreateHippotherapyProgramSectionDto section)
    {
        var req = GetRequirements(section.Template);
        return $"Each description must be between {req.DescriptionLength.Min} and {req.DescriptionLength.Max} characters";
    }
}
