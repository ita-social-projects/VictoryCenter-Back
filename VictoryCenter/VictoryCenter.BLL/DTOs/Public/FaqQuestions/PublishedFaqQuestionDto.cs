namespace VictoryCenter.BLL.DTOs.Public.FaqQuestions;

public record PublishedFaqQuestionDto
{
    public long Id { get; init; }
    public string QuestionText { get; init; } = default!;
    public string AnswerText { get; init; } = default!;
}
