using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Public.HippotherapyPrograms;

public record PublishedHippotherapyProgramDto
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public DateTimeOffset CreatedAt { get; init; }
    public ImageDto? PreviewImage { get; init; }
    public List<ProgramCategoryShortDto> Categories { get; init; } = [];
}
