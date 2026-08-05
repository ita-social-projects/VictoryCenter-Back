namespace VictoryCenter.BLL.DTOs.Admin.Localization.PartnersPageBanner;

public record UpdatePartnersPageBannerLocalizationDto
{
    public string Title { get; init; } = null!;

    public string Description { get; init; } = null!;
}
