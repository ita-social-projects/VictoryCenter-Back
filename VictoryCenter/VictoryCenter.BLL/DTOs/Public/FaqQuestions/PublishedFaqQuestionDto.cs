using VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;

namespace VictoryCenter.BLL.DTOs.Public.FaqQuestions;

public record PublishedFaqQuestionDto
{
    public long Id { get; init; }
    public string QuestionText { get; init; } = null!;
    public string AnswerText { get; init; } = null!;
    public IEnumerable<FaqQuestionLocalizationDto> Localizations { get; init; } = [];
}
