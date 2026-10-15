using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageEthicsSection;

public class HippotherapyLandingPageEthicsPrincipleLocalizationItemDto
{
    public long PrincipleId { get; init; }

    public string Text { get; set; } = null!;

    public TranslationStatus? TranslationStatus { get; init; }
}
