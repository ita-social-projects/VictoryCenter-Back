using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageScientificReferencesSection
    : BaseEntity, ITranslatedEntity<HippotherapyLandingPageScientificReferencesSectionLocalization>
{
    public long HippotherapyLandingPageId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public ICollection<HippotherapyLandingPageScientificReference> ScientificReferences { get; set; } = [];

    public HippotherapyLandingPage HippotherapyLandingPage { get; set; } = null!;

    public ICollection<HippotherapyLandingPageScientificReferencesSectionLocalization> Localizations { get; set; } = [];
}
