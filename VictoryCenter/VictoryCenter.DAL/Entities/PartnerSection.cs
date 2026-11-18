using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class PartnerSection : BaseEntity, IOrderableEntity, ITranslatedEntity<PartnerSectionLocalization>
{
    [Audited]
    public string Title { get; set; } = null!;

    [Audited]
    public string Description { get; set; } = null!;

    [Audited]
    public long Priority { get; set; }

    public ICollection<Partner> Partners { get; set; } = [];

    public ICollection<PartnerSectionLocalization> Localizations { get; set; } = [];
}
