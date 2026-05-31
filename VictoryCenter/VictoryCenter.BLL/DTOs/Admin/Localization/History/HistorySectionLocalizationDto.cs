namespace VictoryCenter.BLL.DTOs.Admin.Localization.History;

public record HistorySectionLocalizationDto
{
    public long EntityId { get; init; }
    public ICollection<HistorySectionContentLocalizationDto> Contents { get; init; } = [];
}
