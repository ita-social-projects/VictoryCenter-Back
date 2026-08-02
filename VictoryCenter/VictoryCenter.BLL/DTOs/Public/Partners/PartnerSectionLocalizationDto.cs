using VictoryCenter.BLL.DTOs.Common;

namespace VictoryCenter.BLL.DTOs.Public.Partners;

public record PartnerSectionLocalizationDto
{
    public LocalizationInfoDto Language { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
}
