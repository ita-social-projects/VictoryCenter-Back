using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;

public record HippotherapyProgramSectionContentLocalizationDto
{
    public long EntityId { get; init; }

    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;

    public string? Title { get; init; }

    public string? Description { get; init; }

    public string? Author { get; init; }

    public string? Question { get; init; }

    public string? Answer { get; init; }
}
