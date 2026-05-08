namespace VictoryCenter.BLL.DTOs.Admin.Localization.History.Create;

public record CreateHistorySectionLocalizationDto
{
    public long EntityId { get; set; }
    public List<CreateHistorySectionContentLocalizationDto> Contents { get; set; } = [];
}
