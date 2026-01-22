using VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Constants;

public static class HippotherapyProgramSectionConstants
{
    public sealed record GroupingConfig(
        (int Min, int Max) GroupCount,
        IReadOnlyDictionary<ContentType, (int Min, int Max)> PerGroupCounts
    );

    public sealed record TemplateRequirementsConfig(
        (int Min, int Max) TitleCount = default,
        (int Min, int Max) TitleLength = default,
        (int Min, int Max) DescriptionCount = default,
        (int Min, int Max) DescriptionLength = default,
        (int Min, int Max) ImageCount = default,
        (int Min, int Max) AuthorCount = default,
        (int Min, int Max) AuthorLength = default,
        (int Min, int Max) QuestionCount = default,
        (int Min, int Max) QuestionLength = default,
        (int Min, int Max) AnswerCount = default,
        (int Min, int Max) AnswerLength = default,
        GroupingConfig? Grouping = null
    );

    private static readonly Dictionary<ProgramSectionTemplate, TemplateRequirementsConfig> TemplateRequirements = new()
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
        ),
        [ProgramSectionTemplate.DualTitleDescriptionPairs] = new(
            TitleCount: (2, 2),
            TitleLength: (5, 60),
            DescriptionCount: (2, 2),
            DescriptionLength: (10, 300),
            ImageCount: (0, 0),
            Grouping: new GroupingConfig(
                GroupCount: (2, 2),
                PerGroupCounts: new Dictionary<ContentType, (int Min, int Max)>
                {
                    [ContentType.Title] = (1, 1),
                    [ContentType.Description] = (1, 1)
                })
        ),
        [ProgramSectionTemplate.TripleTitleDescriptionPairs] = new(
            TitleCount: (3, 3),
            TitleLength: (5, 60),
            DescriptionCount: (3, 3),
            DescriptionLength: (10, 300),
            ImageCount: (0, 0),
            Grouping: new GroupingConfig(
                GroupCount: (3, 3),
                PerGroupCounts: new Dictionary<ContentType, (int Min, int Max)>
                {
                    [ContentType.Title] = (1, 1),
                    [ContentType.Description] = (1, 1)
                })
        ),
        [ProgramSectionTemplate.QuadTitleDescriptionPairs] = new(
            TitleCount: (4, 4),
            TitleLength: (5, 60),
            DescriptionCount: (4, 4),
            DescriptionLength: (10, 300),
            ImageCount: (0, 0),
            Grouping: new GroupingConfig(
                GroupCount: (4, 4),
                PerGroupCounts: new Dictionary<ContentType, (int Min, int Max)>
                {
                    [ContentType.Title] = (1, 1),
                    [ContentType.Description] = (1, 1)
                })
        ),
        [ProgramSectionTemplate.SingleTitleQuintupleDescription] = new(
            TitleCount: (1, 1),
            TitleLength: (5, 60),
            DescriptionCount: (5, 5),
            DescriptionLength: (10, 300),
            ImageCount: (0, 0)
        ),
        [ProgramSectionTemplate.SingleTitleDescriptionAuthorPairs] = new(
            TitleCount: (1, 1),
            TitleLength: (5, 50),
            DescriptionCount: (1, 5),
            DescriptionLength: (10, 100),
            ImageCount: (0, 0),
            AuthorCount: (1, 5),
            AuthorLength: (2, 50),
            Grouping: new GroupingConfig(
                GroupCount: (1, 5),
                PerGroupCounts: new Dictionary<ContentType, (int Min, int Max)>
                {
                    [ContentType.Description] = (1, 1),
                    [ContentType.Author] = (1, 1)
                })
        ),
        [ProgramSectionTemplate.QuestionAnswerPair] = new(
            TitleCount: (1, 1),
            TitleLength: (5, 60),
            QuestionLength: (10, 150),
            AnswerLength: (50, 1000),
            Grouping: new GroupingConfig(
                GroupCount: (1, 10),
                PerGroupCounts: new Dictionary<ContentType, (int Min, int Max)>
                {
                    [ContentType.Question] = (1, 1),
                    [ContentType.Answer] = (1, 1)
                })
        ),
    };
    public static TemplateRequirementsConfig GetRequirements(ProgramSectionTemplate template)
        => TemplateRequirements[template];

    public static string GetGroupCompositionErrorMessage(CreateHippotherapyProgramSectionDto section)
        => $"Template {section.Template} has invalid group composition";

    public static string GetTitlesCountErrorMessage(CreateHippotherapyProgramSectionDto section)
        => GetCountErrorMessage(section, ContentType.Title, GetRequirements(section.Template).TitleCount, "title(s)");

    public static string GetDescriptionsCountErrorMessage(CreateHippotherapyProgramSectionDto section)
        => GetCountErrorMessage(section, ContentType.Description, GetRequirements(section.Template).DescriptionCount, "description(s)");

    public static string GetImagesCountErrorMessage(CreateHippotherapyProgramSectionDto section)
        => GetCountErrorMessage(section, ContentType.Image, GetRequirements(section.Template).ImageCount, "image(s)");

    public static string GetTitleLengthErrorMessage(CreateHippotherapyProgramSectionDto section)
        => GetLengthErrorMessage(GetRequirements(section.Template).TitleLength, "title");

    public static string GetDescriptionLengthErrorMessage(CreateHippotherapyProgramSectionDto section)
        => GetLengthErrorMessage(GetRequirements(section.Template).DescriptionLength, "description");

    public static string GetAuthorsCountErrorMessage(CreateHippotherapyProgramSectionDto section)
        => GetCountErrorMessage(section, ContentType.Author, GetRequirements(section.Template).AuthorCount, "author(s)");

    public static string GetAuthorLengthErrorMessage(CreateHippotherapyProgramSectionDto section)
        => GetLengthErrorMessage(GetRequirements(section.Template).AuthorLength, "author");

    public static string GetQuestionsCountErrorMessage(CreateHippotherapyProgramSectionDto section)
        => GetCountErrorMessage(section, ContentType.Question, GetRequirements(section.Template).QuestionCount, "question(s)");

    public static string GetQuestionLengthErrorMessage(CreateHippotherapyProgramSectionDto section)
        => GetLengthErrorMessage(GetRequirements(section.Template).QuestionLength, "question");

    public static string GetAnswersCountErrorMessage(CreateHippotherapyProgramSectionDto section)
        => GetCountErrorMessage(section, ContentType.Answer, GetRequirements(section.Template).AnswerCount, "answer(s)");

    public static string GetAnswerLengthErrorMessage(CreateHippotherapyProgramSectionDto section)
        => GetLengthErrorMessage(GetRequirements(section.Template).AnswerLength, "answer");

    public static string GetGroupIndexRequiredErrorMessage(CreateHippotherapyProgramSectionDto section)
        => $"Template {section.Template} requires GroupIndex for grouped content";

    public static string GetGroupCountErrorMessage(CreateHippotherapyProgramSectionDto section, int actual)
    {
        var req = GetRequirements(section.Template).Grouping!;

        return $"Template {section.Template} requires between {req.GroupCount.Min} and {req.GroupCount.Max} group(s), but received {actual}";
    }

    private static int CountByType(CreateHippotherapyProgramSectionDto section, ContentType type)
        => (section.Contents ?? []).Count(c => c.ContentType == type);

    private static string GetCountErrorMessage(
        CreateHippotherapyProgramSectionDto section,
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
