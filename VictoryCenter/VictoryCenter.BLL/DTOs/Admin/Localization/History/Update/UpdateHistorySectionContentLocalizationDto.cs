using VictoryCenter.BLL.DTOs.Admin.Localization.History.Common;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.History.Update;

public record UpdateHistorySectionContentLocalizationDto : IHistoryContentLocalization
{
    public long EntityId { get; set; }
    public string? Title { get; init; }
    public string? Description { get; init; }
}
