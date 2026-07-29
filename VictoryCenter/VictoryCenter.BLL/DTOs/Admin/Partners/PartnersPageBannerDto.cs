using VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;
using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record PartnersPageBannerDto
{
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public ImageDto? Image { get; init; }
    public List<PartnersPageBannerLocalizationDto> Localizations { get; init; } = [];
}
