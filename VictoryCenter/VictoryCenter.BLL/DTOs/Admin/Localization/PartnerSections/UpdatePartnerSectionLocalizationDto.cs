namespace VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;

public record UpdatePartnerSectionLocalizationDto
{
    public string Title { get; init; } = null!;

    public string Description { get; init; } = null!;

    public List<UpdatePartnerLocalizationItemDto> Partners { get; init; } = [];
}
