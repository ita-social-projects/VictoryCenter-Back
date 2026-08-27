using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;

namespace VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;

public class HippotherapyLandingPageScientificReference : BaseEntity, IOrderableEntity
{
    public long ScientificReferencesSectionId { get; set; }

    public string Name { get; set; } = null!;

    public string Url { get; set; } = null!;

    public long Priority { get; set; }

    public HippotherapyLandingPageScientificReferencesSection ScientificReferencesSection { get; set; } = null!;
}
