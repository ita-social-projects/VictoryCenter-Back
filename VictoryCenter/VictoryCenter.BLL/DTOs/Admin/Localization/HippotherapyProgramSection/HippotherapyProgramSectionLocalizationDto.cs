namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;

public record HippotherapyProgramSectionLocalizationDto
{
    public long EntityId { get; init; }

    public List<HippotherapyProgramSectionContentLocalizationDto> Contents { get; set; } = [];
}
