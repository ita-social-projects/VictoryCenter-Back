using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;

public record HippotherapyProgramDto
{
    public long Id { get; init; }

    public string Name { get; init; } = null!;

    public string? Description { get; init; }

    public Status Status { get; init; }

    public ImageDto? Image { get; init; }
    public List<ProgramCategoryShortDto> Categories { get; init; } = [];
}
