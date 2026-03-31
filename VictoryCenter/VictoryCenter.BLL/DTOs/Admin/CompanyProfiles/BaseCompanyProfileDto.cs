using VictoryCenter.BLL.DTOs.Admin.CompanyProfileContacts;
using VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisites;
using VictoryCenter.BLL.DTOs.Admin.SocialLinks;

namespace VictoryCenter.BLL.DTOs.Admin.CompanyProfiles;

public class BaseCompanyProfileDto<TContacts, TRequisites, TSocialLink>
    where TContacts : BaseCompanyProfileContactsDto
    where TRequisites : BaseCompanyProfileRequisiteDto
    where TSocialLink : BaseSocialLinkDto
{
    public TContacts Contacts { get; set; } = null!;
    public TRequisites Requisites { get; set; } = null!;
    public List<TSocialLink> SocialLinks { get; set; } = [];
}
