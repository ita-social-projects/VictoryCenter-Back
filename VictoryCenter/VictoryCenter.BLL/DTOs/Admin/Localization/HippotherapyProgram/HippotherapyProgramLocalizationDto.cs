using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;

public record HippotherapyProgramLocalizationDto : BaseHippotherapyProgramLocalizationDto
{
    public long EntityId { get; init; }

    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;

    public TranslationStatus TranslationStatus { get; init; }

    public TranslationStatus TranslationStatus { get; init; }

    public List<HippotherapyProgramSectionLocalizationDto> Sections { get; init; } = [];
}
