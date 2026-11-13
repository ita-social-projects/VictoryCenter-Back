using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

[Auditable]
public class HippotherapyLandingPageScientificReference
    : BaseEntity, IOrderableEntity, ITranslatedEntity<HippotherapyLandingPageScientificReferenceLocalization>
{
    [Audited]
    public long ScientificReferencesSectionId { get; set; }

    [Audited]
    public string Name { get; set; } = null!;

    [Audited]
    public string Url { get; set; } = null!;

    [Audited]
    public long Priority { get; set; }

    public HippotherapyLandingPageScientificReferencesSection ScientificReferencesSection { get; set; } = null!;

    public ICollection<HippotherapyLandingPageScientificReferenceLocalization> Localizations { get; set; } = [];
}
