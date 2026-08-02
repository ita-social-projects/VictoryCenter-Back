namespace VictoryCenter.BLL.DTOs.Public.Partners;

public record PartnersPageDto
{
    public PublicPartnersPageBannerDto Banner { get; set; } = null!;
    public IEnumerable<PublicPartnersSectionDto> Sections { get; set; } = null!;
}
