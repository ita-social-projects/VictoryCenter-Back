using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Update;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;

public record UpdateHippotherapyProgramLocalizationDto : BaseHippotherapyProgramLocalizationDto
{
    public List<UpdateHippotherapyProgramSectionLocalizationDto> Sections { get; init; } = [];
}
