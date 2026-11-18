using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

[Auditable]
public class HippotherapyLandingPageAdvantagesSection
    : BaseEntity, ITranslatedEntity<HippotherapyLandingPageAdvantagesSectionLocalization>
{
    [Audited]
    public long HippotherapyLandingPageId { get; set; }

    [Audited]
    [RichText]
    public string Title { get; set; } = null!;

    public ICollection<HippotherapyLandingPageAdvantageCard> AdvantageCards { get; set; } = [];

    public HippotherapyLandingPage HippotherapyLandingPage { get; set; } = null!;

    public ICollection<HippotherapyLandingPageAdvantagesSectionLocalization> Localizations { get; set; } = [];
}
