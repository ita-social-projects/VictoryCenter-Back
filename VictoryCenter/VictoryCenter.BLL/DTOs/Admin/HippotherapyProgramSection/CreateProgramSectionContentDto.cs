using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyProgramSection;

public record CreateProgramSectionContentDto
{
    public long? Id { get; init; }

    public ContentType ContentType { get; init; }

    public int Order { get; init; }

    public int? GroupIndex { get; init; }

    public string? Title { get; init; }

    public string? Description { get; init; }

    public long? ImageId { get; init; }

    public string? Author { get; init; }

    public CreateFaqSectionQuestionDto? FaqQuestion { get; init; }
}
