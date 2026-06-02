namespace VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

public record UpdateMainPageLocalizationDto : BaseMainPageLocalizationDto
{
    public UpdateMainAboutUsLocalizationDto? MainAboutUs { get; init; }
    public UpdateMainPartnersLocalizationDto? MainPartners { get; init; }
    public UpdateMainDonationsLocalizationDto? MainDonations { get; init; }
}
