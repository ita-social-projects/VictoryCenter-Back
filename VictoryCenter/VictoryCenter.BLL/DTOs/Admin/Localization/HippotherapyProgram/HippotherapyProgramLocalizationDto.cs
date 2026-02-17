using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramSection;
using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgram;

public class HippotherapyProgramLocalizationDto
{
    public long Id { get; init; }

    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;

    public string Name { get; init; } = null!;

    public string? Description { get; init; }

    public string? Location { get; init; }

    public string? ParticipantsCount { get; init; }

    public string? MeetingsCount { get; init; }

    public List<HippotherapyProgramSectionLocalizationDto> Sections { get; init; } = [];

    public TranslationStatus TranslationStatus { get; init; }
}
