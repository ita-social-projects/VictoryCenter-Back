using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageEthicsSection : BaseEntity
{
    public long HippotherapyLandingPageId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public long? ImageId { get; set; }

    public Image? Image { get; set; }

    public ICollection<HippotherapyLandingPageEthicsPrinciple> EthicsPrinciples { get; set; } = [];

    public HippotherapyLandingPage HippotherapyLandingPage { get; set; } = null!;
}
