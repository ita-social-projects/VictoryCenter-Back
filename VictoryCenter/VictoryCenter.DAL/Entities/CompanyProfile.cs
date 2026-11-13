using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class CompanyProfile : BaseEntity
{
    public CompanyProfileContact Contact { get; set; } = null!;

    public CompanyProfileRequisite Requisite { get; set; } = null!;

    public ICollection<CompanyProfileSocialLink> SocialLinks { get; set; } = [];
}
