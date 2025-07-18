using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.FaqQuestions;

public record FaqQuestionDto
{
    public long Id { get; init; }

    public string QuestionText { get; init; } = null!;

    public string AnswerText { get; init; } = null!;

    public Status Status { get; init; }

    public List<long> PageIds { get; init; } = [];
}
