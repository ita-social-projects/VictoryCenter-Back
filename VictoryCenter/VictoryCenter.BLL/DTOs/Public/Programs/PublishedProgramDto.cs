using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Public.Programs;

public record PublishedProgramDto
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public ImageDto? Image { get; init; }
    public List<ProgramCategoryShortDto> Categories { get; init; } = [];
}
