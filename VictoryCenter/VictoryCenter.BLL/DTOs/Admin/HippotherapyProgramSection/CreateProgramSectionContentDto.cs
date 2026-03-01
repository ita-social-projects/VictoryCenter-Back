using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;

public record CreateProgramSectionContentDto
{
    public ContentType ContentType { get; init; }

    public int Order { get; init; }

    public int? GroupIndex { get; init; }

    public string? Title { get; init; }

    public string? Description { get; init; }

    public long? ImageId { get; init; }

    public string? Author { get; init; }

    public string? QuestionText { get; init; }

    public string? AnswerText { get; init; }
}
