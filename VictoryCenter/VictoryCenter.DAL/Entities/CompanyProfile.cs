using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class CompanyProfile : BaseEntity
{
    public CompanyProfileContact Contact { get; set; } = null!;
    public CompanyProfileRequisite Requisite { get; set; } = null!;

    public ICollection<CompanyProfileSocialLink> SocialLinks { get; set; } = [];
}
