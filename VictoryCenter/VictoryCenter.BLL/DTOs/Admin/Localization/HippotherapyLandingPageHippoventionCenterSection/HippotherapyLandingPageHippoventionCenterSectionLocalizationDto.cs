using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageHippoventionCenterSection;

public class HippotherapyLandingPageHippoventionCenterSectionLocalizationDto
{
    public long EntityId { get; init; }

    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Pros { get; set; } = null!;

    public TranslationStatus TranslationStatus { get; init; }
}
