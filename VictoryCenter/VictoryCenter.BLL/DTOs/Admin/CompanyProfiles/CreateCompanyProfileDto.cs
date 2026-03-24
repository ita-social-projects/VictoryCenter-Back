using VictoryCenter.BLL.DTOs.Admin.CompanyProfileContacts;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisite;
using VictoryCenter.BLL.DTOs.Admin.Localization.CompanyProfile;
using VictoryCenter.BLL.DTOs.Admin.SocialLinks;

namespace VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;

public class CreateCompanyProfileDto
{
    public CreateCompanyProfileContactsDto Contacts { get; set; } = null!;
    public CreateCompanyProfileRequisiteDto Requisites { get; set; } = null!;
    public List<CreateSocialLinkDto> SocialLinks { get; set; } = [];
}
