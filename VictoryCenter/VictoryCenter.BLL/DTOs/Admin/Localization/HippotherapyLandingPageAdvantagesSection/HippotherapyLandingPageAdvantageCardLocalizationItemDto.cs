using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAdvantagesSection;

public class HippotherapyLandingPageAdvantageCardLocalizationItemDto
{
    public long CardId { get; init; }

    public string Description { get; set; } = null!;

    public TranslationStatus? TranslationStatus { get; init; }
}
