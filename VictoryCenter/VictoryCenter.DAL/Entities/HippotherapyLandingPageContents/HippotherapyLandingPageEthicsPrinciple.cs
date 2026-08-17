using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageEthicsPrinciple : BaseEntity
{
    public long EthicsSectionId { get; set; }

    public string Text { get; set; } = null!;

    public long Priority { get; set; }

    public HippotherapyLandingPageEthicsSection EthicsSection { get; set; } = null!;
}
