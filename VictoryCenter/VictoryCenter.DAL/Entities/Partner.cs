using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class Partner : BaseEntity, IOrderableEntity, ITranslatedEntity<PartnerLocalization>
{
    [Audited]
    public long PartnersSectionId { get; set; }

    [Audited]
    public string Description { get; set; } = null!;

    [Audited]
    public long Priority { get; set; }

    [Audited]
    public long ImageId { get; set; }

    public Image Image { get; set; } = null!;

    public PartnerSection PartnerSection { get; set; } = null!;

    public ICollection<PartnerLocalization> Localizations { get; set; } = [];
}
