using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities.WhoWeAreContents;

[Auditable]
public abstract class WhoWeAreContent : ITranslatedEntity<WhoWeAreContentLocalization>, IEntity
{
    public long Id { get; set; }

    [Audited]
    public long SectionId { get; set; }

    [Audited]
    public ContentType ContentType { get; set; }

    public WhoWeAreSection Section { get; set; } = null!;

    public ICollection<WhoWeAreContentLocalization> Localizations { get; set; } = [];
}
