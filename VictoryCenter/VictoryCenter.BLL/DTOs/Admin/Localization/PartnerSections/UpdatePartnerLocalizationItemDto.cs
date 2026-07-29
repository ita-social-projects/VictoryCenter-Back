namespace VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;

public record UpdatePartnerLocalizationItemDto
{
    public long PartnerId { get; init; }

    public string Description { get; init; } = null!;
}
