using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;

public record PartnerLocalizationItemDto
{
    public long PartnerId { get; init; }

    public string Description { get; init; } = null!;

    public TranslationStatus? TranslationStatus { get; init; }
}
