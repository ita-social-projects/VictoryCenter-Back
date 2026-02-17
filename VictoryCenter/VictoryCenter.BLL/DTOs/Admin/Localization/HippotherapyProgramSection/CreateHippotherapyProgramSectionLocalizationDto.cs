namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;

public class CreateHippotherapyProgramSectionLocalizationDto
{
    public long EntityId { get; }

    public List<CreateHippotherapyProgramSectionContentLocalizationDto>? Contents { get; set; } = [];
}
