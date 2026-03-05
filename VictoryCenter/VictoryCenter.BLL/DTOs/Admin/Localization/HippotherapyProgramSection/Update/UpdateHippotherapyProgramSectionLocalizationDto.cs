namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Update;
public record UpdateHippotherapyProgramSectionLocalizationDto
{
    public long EntityId { get; set; }

    public List<UpdateHippotherapyProgramSectionContentLocalizationDto>? Contents { get; set; } = [];
}
