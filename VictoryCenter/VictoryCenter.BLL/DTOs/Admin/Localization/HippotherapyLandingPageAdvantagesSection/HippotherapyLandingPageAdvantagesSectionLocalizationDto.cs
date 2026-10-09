using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;

public class HippotherapyLandingPageAdvantagesSectionLocalizationDto
{
    public long EntityId { get; init; }

    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;

    public string Title { get; set; } = null!;

    public TranslationStatus TranslationStatus { get; init; }

    public List<HippotherapyLandingPageAdvantageCardLocalizationItemDto> Cards { get; set; } = [];
}
