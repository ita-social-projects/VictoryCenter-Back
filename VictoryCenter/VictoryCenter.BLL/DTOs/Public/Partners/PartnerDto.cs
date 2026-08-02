using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Public.Partners;

public record PartnerDto
{
    public long Id { get; init; }
    public string Description { get; init; } = null!;
    public ImageDto Image { get; init; } = null!;
    public List<PartnerLocalizationDto> Localizations { get; init; } = [];
}
