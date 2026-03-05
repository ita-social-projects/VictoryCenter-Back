using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Common;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Update;
public record UpdateHippotherapyProgramSectionContentLocalizationDto : BaseHippotherapyProgramSectionContentLocalizationDto
{
    public int EntityId { get; set; }
}
