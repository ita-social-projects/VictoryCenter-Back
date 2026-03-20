using VictoryCenter.BLL.DTOs.Admin.Localization.Base;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection.Common;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;

public record CreateHippotherapyProgramSectionContentLocalizationDto : BaseHippotherapyProgramSectionContentLocalizationDto, ILocalizationIdentity
{
    public long EntityId { get; init; }
    public long LanguageId { get; init; }
}
