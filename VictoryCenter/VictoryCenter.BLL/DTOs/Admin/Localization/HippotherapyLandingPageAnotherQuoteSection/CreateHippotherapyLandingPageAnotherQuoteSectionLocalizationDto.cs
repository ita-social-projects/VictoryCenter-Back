using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageAnotherQuoteSection;

public class CreateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto
    : UpdateHippotherapyLandingPageAnotherQuoteSectionLocalizationDto, ILocalizationIdentity
{
    public long EntityId { get; init; }

    public long LanguageId { get; init; }
}
