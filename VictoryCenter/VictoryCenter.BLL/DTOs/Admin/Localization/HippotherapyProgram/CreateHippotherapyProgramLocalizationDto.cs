using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;

public record CreateHippotherapyProgramLocalizationDto : UpdateHippotherapyProgramLocalizationDto,
    ILocalizationIdentity
{
    public long EntityId { get; init; }

    public long LanguageId { get; init; }
}
