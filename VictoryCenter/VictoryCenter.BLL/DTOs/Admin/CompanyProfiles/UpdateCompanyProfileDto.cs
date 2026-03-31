using VictoryCenter.BLL.DTOs.Admin.CompanyProfileContacts;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisites;
using VictoryCenter.BLL.DTOs.Admin.SocialLinks;

namespace VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;

public class UpdateCompanyProfileDto
    : BaseCompanyProfileDto<UpdateCompanyProfileContactDto, UpdateCompanyProfileRequisiteDto, UpdateSocialLinkDto>
{
}
