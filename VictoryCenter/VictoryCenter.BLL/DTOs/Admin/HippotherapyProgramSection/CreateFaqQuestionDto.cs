namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;

public record CreateFaqQuestionDto
{
    public long? Id { get; init; }

    public string QuestionText { get; init; } = null!;

    public string AnswerText { get; init; } = null!;
}
