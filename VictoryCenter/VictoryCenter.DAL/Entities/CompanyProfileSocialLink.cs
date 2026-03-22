using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class CompanyProfileSocialLink : BaseEntity
{
    public long ProfileId { get; set; }
    public CompanyProfile Profile { get; set; } = null!;

    public SocialPlatform SocialPlatform { get; set; }
    public string Url { get; set; } = null!;
}
