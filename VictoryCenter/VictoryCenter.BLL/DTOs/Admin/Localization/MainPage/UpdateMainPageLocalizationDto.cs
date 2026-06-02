using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage.Metrics;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

public record UpdateMainPageLocalizationDto : BaseMainPageLocalizationDto
{
    public UpdateMainAboutUsLocalizationDto? MainAboutUs { get; init; }
    public UpdateMainPartnersLocalizationDto? MainPartners { get; init; }
    public UpdateMainDonationsLocalizationDto? MainDonations { get; init; }
    public UpdateImpactStatisticLocalizationDto? ImpactStatistics { get; init; }
    public ICollection<UpdateMetricLocalizationDto> Metrics { get; init; } = [];
}
