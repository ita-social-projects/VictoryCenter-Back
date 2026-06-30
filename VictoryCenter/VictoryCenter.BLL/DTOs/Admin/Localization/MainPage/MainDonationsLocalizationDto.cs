using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

public record MainDonationsLocalizationDto : BaseMainPageLocalizationDto
{
    public long EntityId { get; init; }
    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;
    public TranslationStatus TranslationStatus { get; init; }
}
