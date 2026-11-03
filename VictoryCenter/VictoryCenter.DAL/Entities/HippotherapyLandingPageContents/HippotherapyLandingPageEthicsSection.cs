using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageEthicsSection
    : BaseEntity, ITranslatedEntity<HippotherapyLandingPageEthicsSectionLocalization>
{
    public long HippotherapyLandingPageId { get; set; }

    [RichText]
    public string Title { get; set; } = null!;

    [RichText]
    public string Description { get; set; } = null!;

    public long? ImageId { get; set; }

    public Image? Image { get; set; }

    public ICollection<HippotherapyLandingPageEthicsPrinciple> EthicsPrinciples { get; set; } = [];

    public HippotherapyLandingPage HippotherapyLandingPage { get; set; } = null!;

    public ICollection<HippotherapyLandingPageEthicsSectionLocalization> Localizations { get; set; } = [];
}
