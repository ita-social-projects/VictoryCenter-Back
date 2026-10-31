using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageDescriptionSection
    : BaseEntity, ITranslatedEntity<HippotherapyLandingPageDescriptionSectionLocalization>
{
    public long HippotherapyLandingPageId { get; set; }

    [RichText]
    public string Title { get; set; } = null!;

    [RichText]
    public string Description { get; set; } = null!;

    public HippotherapyLandingPage HippotherapyLandingPage { get; set; } = null!;

    public ICollection<HippotherapyLandingPageDescriptionSectionLocalization> Localizations { get; set; } = [];
}
