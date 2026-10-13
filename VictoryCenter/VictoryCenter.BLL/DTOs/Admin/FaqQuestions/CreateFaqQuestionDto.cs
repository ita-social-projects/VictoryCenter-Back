using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.FaqQuestions;

public record CreateFaqQuestionDto
{
    public string QuestionText { get; init; } = null!;

    public string? AnswerText { get; init; }

    public Status Status { get; init; }

    public List<long> PageIds { get; init; } = [];
}
