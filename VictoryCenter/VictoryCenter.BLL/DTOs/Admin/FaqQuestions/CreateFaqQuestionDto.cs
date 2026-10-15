using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.FaqQuestions;

public record CreateFaqQuestionDto
{
    private string? _answerText;
    public string QuestionText { get; init; } = null!;

    public string? AnswerText
    {
        get => _answerText;
        init => _answerText = value?.Trim();
    }

    public Status Status { get; init; }

    public List<long> PageIds { get; init; } = [];
}
