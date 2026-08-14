using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Public.Partners;

public record PublicPartnersPageBannerDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public ImageDto? Image { get; init; }
    public List<PublicPartnersPageBannerLocalizationDto> Localizations { get; init; } = [];
}
