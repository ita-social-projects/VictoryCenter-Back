namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;

public class CreateHippotherapyProgramSectionLocalizationDto
{
    public long EntityId { get; init; }

    public List<CreateHippotherapyProgramSectionContentLocalizationDto>? Contents { get; set; } = [];
}
