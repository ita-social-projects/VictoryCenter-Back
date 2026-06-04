namespace VictoryCenter.BLL.DTOs.Admin.Localization.History.Update;

public record UpdateHistorySectionLocalizationDto
{
    public long EntityId { get; set; }
    public List<UpdateHistorySectionContentLocalizationDto> Contents { get; set; } = [];
}
