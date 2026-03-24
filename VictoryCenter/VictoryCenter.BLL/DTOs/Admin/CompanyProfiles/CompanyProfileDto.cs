using VictoryCenter.BLL.DTOs.Admin.CompanyProfileContacts;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisite;
using VictoryCenter.BLL.DTOs.Admin.SocialLinks;

namespace VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;

public record CompanyProfileDto
{
    public CompanyProfileContactsDto Contacts { get; set; } = null!;
    public CompanyProfileRequisiteDto Requisites { get; set; } = null!;
    public List<SocialLinkDto> SocialLinks { get; set; } = [];
}
