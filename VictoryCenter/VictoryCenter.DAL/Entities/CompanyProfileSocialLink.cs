using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class CompanyProfileSocialLink : BaseEntity
{
    [Audited]
    public long ProfileId { get; set; }

    public CompanyProfile Profile { get; set; } = null!;

    [Audited]
    public SocialPlatform SocialPlatform { get; set; }

    [Audited]
    public string Url { get; set; } = null!;
}
