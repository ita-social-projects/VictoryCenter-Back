using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageScientificReferencesSection;

public class HippotherapyLandingPageScientificReferenceLocalizationItemDto
{
    public long ReferenceId { get; init; }

    public string Name { get; set; } = null!;

    public TranslationStatus? TranslationStatus { get; init; }
}
