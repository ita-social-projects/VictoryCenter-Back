using VictoryCenter.BLL.DTOs.Common;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;

public record PartnersPageBannerLocalizationDto
{
    public long EntityId { get; init; }

    public LocalizationInfoDto LocalizationInfoDto { get; init; } = null!;

    public string Title { get; init; } = null!;

    public string Description { get; init; } = null!;

    public TranslationStatus TranslationStatus { get; init; }
}
