using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Public.Partners;

public record PartnerLocalizationDto
{
    public LocalizationInfoDto Language { get; init; } = null!;
    public string Description { get; init; } = null!;
}
