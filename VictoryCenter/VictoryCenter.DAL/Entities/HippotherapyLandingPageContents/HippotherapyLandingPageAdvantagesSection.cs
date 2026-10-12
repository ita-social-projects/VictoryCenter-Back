using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageAdvantagesSection
    : BaseEntity, ITranslatedEntity<HippotherapyLandingPageAdvantagesSectionLocalization>
{
    public long HippotherapyLandingPageId { get; set; }

    public string Title { get; set; } = null!;

    public ICollection<HippotherapyLandingPageAdvantageCard> AdvantageCards { get; set; } = [];

    public HippotherapyLandingPage HippotherapyLandingPage { get; set; } = null!;

    public ICollection<HippotherapyLandingPageAdvantagesSectionLocalization> Localizations { get; set; } = [];
}
