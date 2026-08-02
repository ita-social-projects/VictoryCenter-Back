namespace VictoryCenter.BLL.DTOs.Public.Partners;

public record PartnersPageDto
{
    public PartnersPageBannerDto Banner { get; set; } = null!;
    public IEnumerable<PartnersSectionDto> Sections { get; set; } = null!;
}
