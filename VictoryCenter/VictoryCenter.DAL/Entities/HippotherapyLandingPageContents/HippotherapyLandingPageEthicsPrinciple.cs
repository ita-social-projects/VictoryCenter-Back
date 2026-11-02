using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageEthicsPrinciple
    : BaseEntity, ITranslatedEntity<HippotherapyLandingPageEthicsPrincipleLocalization>
{
    public long EthicsSectionId { get; set; }

    [RichText]
    public string Text { get; set; } = null!;

    public long Priority { get; set; }

    public HippotherapyLandingPageEthicsSection EthicsSection { get; set; } = null!;

    public ICollection<HippotherapyLandingPageEthicsPrincipleLocalization> Localizations { get; set; } = [];
}
