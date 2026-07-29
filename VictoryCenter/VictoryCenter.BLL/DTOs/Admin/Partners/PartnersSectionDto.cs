using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;

namespace VictoryCenter.BLL.DTOs.Admin.Partners;

public record PartnersSectionDto
{
    public long Id { get; init; }
    public string Title { get; init; } = null!;
    public string Description { get; init; } = null!;
    public List<PartnerDto> Partners { get; init; } = [];
    public List<PartnerSectionLocalizationSummaryDto> Localizations { get; init; } = [];
}
