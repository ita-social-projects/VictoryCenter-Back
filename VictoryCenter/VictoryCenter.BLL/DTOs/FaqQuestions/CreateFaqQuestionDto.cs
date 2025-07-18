using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.FaqQuestions;

public record CreateFaqQuestionDto
{
    public string QuestionText { get; init; } = null!;

    public string AnswerText { get; init; } = null!;

    public Status Status { get; init; }

    public List<long> PageIds { get; init; } = [];
}
