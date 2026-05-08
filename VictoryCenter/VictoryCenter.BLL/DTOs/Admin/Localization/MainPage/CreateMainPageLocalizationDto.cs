using VictoryCenter.BLL.DTOs.Admin.Localization.Base;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

public record CreateMainPageLocalizationDto : BaseMainPageLocalizationDto, ILocalizationIdentity
{
    public long EntityId { get; init; }
    public long LanguageId { get; init; }
    public CreateMainAboutUsLocalizationDto? MainAboutUs { get; init; }
    public CreateMainPartnersLocalizationDto? MainPartners { get; init; }
}
