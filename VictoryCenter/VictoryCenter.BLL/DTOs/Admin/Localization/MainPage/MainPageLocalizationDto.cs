using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

public record MainPageLocalizationDto : BaseMainPageLocalizationDto
{
    public long EntityId { get; init; }
    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;
    public TranslationStatus TranslationStatus { get; init; }
    public MainAboutUsLocalizationDto? MainAboutUs { get; init; }
    public MainPartnersLocalizationDto? MainPartners { get; init; }
}
