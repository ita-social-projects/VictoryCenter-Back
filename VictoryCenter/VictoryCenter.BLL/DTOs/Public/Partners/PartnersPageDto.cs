using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.DTOs.Public.Partners;

public record PartnersPageDto
{
    public PartnersPageBannerDto Banner { get; set; }
    public IEnumerable<PartnersSectionDto> Sections { get; set; }
}
