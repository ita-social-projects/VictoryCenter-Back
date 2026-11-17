using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class HistorySection : BaseEntity
{
    [Audited]
    public HistorySectionTemplate Template { get; set; }

    [Audited]
    public int Order { get; set; }

    public ICollection<HistorySectionContent> Contents { get; set; } = null!;
}
