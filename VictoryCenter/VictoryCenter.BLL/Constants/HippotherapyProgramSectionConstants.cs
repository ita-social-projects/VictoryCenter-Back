using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Constants;

public static class ProgramSectionConstants
{
    public static readonly string SectionOrdersMustBeUnique = "Section orders must be unique";
    public static readonly string SectionOrdersMustBeSequential = "Section orders must be sequential starting from 0";

    public static readonly Dictionary<ProgramSectionTemplate, (
        (int Min, int Max) TitleCount,
        (int Min, int Max) TitleLength,
        (int Min, int Max) DescriptionCount,
        (int Min, int Max) DescriptionLength,
        (int Min, int Max) ImageCount
    )> TemplateRequirements = new()
    {
        [ProgramSectionTemplate.QuadImagesBottom] = (
            TitleCount: (1, 1),
            TitleLength: (5, 60),
            DescriptionCount: (1, 1),
            DescriptionLength: (10, 600),
            ImageCount: (4, 4)
        ),
        [ProgramSectionTemplate.DualImagesBottom] = (
            TitleCount: (1, 1),
            TitleLength: (5, 60),
            DescriptionCount: (1, 1),
            DescriptionLength: (10, 600),
            ImageCount: (2, 2)
        ),
        [ProgramSectionTemplate.TextOnly] = (
            TitleCount: (1, 1),
            TitleLength: (5, 60),
            DescriptionCount: (1, 1),
            DescriptionLength: (10, 600),
            ImageCount: (0, 0)
        ),
        [ProgramSectionTemplate.TripleImagesBottom] = (
            TitleCount: (1, 1),
            TitleLength: (5, 60),
            DescriptionCount: (1, 1),
            DescriptionLength: (10, 600),
            ImageCount: (3, 3)
        ),
        [ProgramSectionTemplate.SingleImageBottom] = (
            TitleCount: (1, 1),
            TitleLength: (5, 60),
            DescriptionCount: (1, 1),
            DescriptionLength: (10, 600),
            ImageCount: (1, 1)
        ),
        [ProgramSectionTemplate.SingleImageTop] = (
            TitleCount: (1, 1),
            TitleLength: (5, 60),
            DescriptionCount: (1, 1),
            DescriptionLength: (10, 600),
            ImageCount: (1, 1)
        ),
        [ProgramSectionTemplate.SingleImageRight] = (
            TitleCount: (1, 1),
            TitleLength: (5, 60),
            DescriptionCount: (1, 1),
            DescriptionLength: (10, 600),
            ImageCount: (1, 1)
        )
    };

    public static string NoValidationRulesDefinedForTemplate(ProgramSectionTemplate template)
    {
        return $"No validation rules defined for template: {template}";
    }

    public static string TemplateRequiresExactlyNTitles(ProgramSectionTemplate template, int required, int actual)
    {
        return $"Template {template} requires exactly {required} title(s), but received {actual}";
    }

    public static string TemplateRequiresExactlyNDescriptions(ProgramSectionTemplate template, int required, int actual)
    {
        return $"Template {template} requires exactly {required} description(s), but received {actual}";
    }

    public static string TemplateRequiresExactlyNImages(ProgramSectionTemplate template, int required, int actual)
    {
        return $"Template {template} requires exactly {required} image(s), but received {actual}";
    }

    public static string TitleMustBeBetweenNAndMCharacters(int min, int max)
    {
        return $"Each title must be between {min} and {max} characters";
    }

    public static string DescriptionMustBeBetweenNAndMCharacters(int min, int max)
    {
        return $"Each description must be between {min} and {max} characters";
    }
}
