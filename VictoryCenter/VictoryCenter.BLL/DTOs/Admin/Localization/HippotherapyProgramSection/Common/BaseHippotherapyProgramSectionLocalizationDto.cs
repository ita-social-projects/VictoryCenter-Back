namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Common;
public abstract class BaseHippotherapyProgramSectionLocalizationDto<TContent>
    where TContent : BaseHippotherapyProgramSectionContentLocalizationDto
{
    public long EntityId { get; init; }
    public List<TContent>? Contents { get; set; } = [];
}
