using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.HippotherapyPrograms;

public class PublishedHippotherapyProgramLocalizationDto
{
    public long EntityId { get; init; }

    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;

    public string Name { get; init; } = null!;

    public string? Description { get; init; }
}
