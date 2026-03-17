using VictoryCenter.BLL.DTOs.Admin.Localization.Base;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;

public record CreateHippotherapyProgramLocalizationDto : BaseHippotherapyProgramLocalizationDto, ILocalizationIdentity
{
    public long EntityId { get; init; }

    public long LanguageId { get; init; }

    public List<CreateHippotherapyProgramSectionLocalizationDto> Sections { get; set; } = [];
}
