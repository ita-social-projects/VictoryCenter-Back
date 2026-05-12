namespace VictoryCenter.BLL.DTOs.Admin.Localization.History.Update;

public record UpdateHistorySectionLocalizationDto
{
    public List<UpdateHistorySectionContentLocalizationDto> Contents { get; set; } = [];
}
