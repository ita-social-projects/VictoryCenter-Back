using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageAnalysisSection : BaseEntity
{
    public long HippotherapyLandingPageId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public HippotherapyLandingPage HippotherapyLandingPage { get; set; } = null!;
}
