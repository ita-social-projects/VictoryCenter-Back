using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageHippoventionPro : BaseEntity
{
    public long HippoventionCenterSectionId { get; set; }

    public string Text { get; set; } = null!;

    public long Priority { get; set; }

    public HippotherapyLandingPageHippoventionCenterSection HippoventionCenterSection { get; set; } = null!;
}
