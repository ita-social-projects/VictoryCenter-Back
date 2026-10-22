using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.EventsIntroSection;

public class EventsIntroSectionLocalizationDto
{
    public long EntityId { get; init; }

    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;

    public string? EventsBlockTitle { get; set; }

    public string? PageDescription { get; set; }

    public TranslationStatus TranslationStatus { get; init; }
}
