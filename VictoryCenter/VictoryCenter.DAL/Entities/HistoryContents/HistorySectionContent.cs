using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities.HistoryContents;

public abstract class HistorySectionContent : IEntity
{
    public long Id { get; set; }

    public long SectionId { get; set; }

    public ContentType ContentType { get; set; }

    public int Order { get; set; }

    public HistorySection Section { get; set; } = null!;
}
