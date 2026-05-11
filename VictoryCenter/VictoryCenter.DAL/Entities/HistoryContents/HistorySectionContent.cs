using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities.HistoryContents;

public abstract class HistorySectionContent : IEntity, ITranslatedEntity<HistorySectionContentLocalization>
{
    public long Id { get; set; }

    public long SectionId { get; set; }

    public ContentType ContentType { get; set; }

    public int Order { get; set; }

    public HistorySection Section { get; set; } = null!;

    public ICollection<HistorySectionContentLocalization> Localizations { get; set; } = [];
}
