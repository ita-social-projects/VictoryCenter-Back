using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;

public record CreateHippotherapyProgramSectionContentLocalizationDto : UpdateHippotherapyProgramSectionContentLocalizationDto, ILocalizationIdentity
{
    public long EntityId { get; init; }
    public long LanguageId { get; }
}
