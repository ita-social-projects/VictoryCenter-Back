namespace VictoryCenter.BLL.DTOs.Admin.Localization.FaqQuestions;

public record UpdateFaqQuestionLocalizationDto
{
    public string QuestionText { get; init; } = null!;

    public string AnswerText { get; init; } = null!;
}
