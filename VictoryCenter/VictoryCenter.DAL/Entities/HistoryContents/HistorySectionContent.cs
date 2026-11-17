using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities.HistoryContents;

[Auditable]
public abstract class HistorySectionContent : IEntity, ITranslatedEntity<HistorySectionContentLocalization>
{
    public long Id { get; set; }

    [Audited]
    public long SectionId { get; set; }

    [Audited]
    public ContentType ContentType { get; set; }

    [Audited]
    public int Order { get; set; }

    public HistorySection Section { get; set; } = null!;

    public ICollection<HistorySectionContentLocalization> Localizations { get; set; } = [];
}
