using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.HistoryContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class HistorySection : BaseEntity
{
    public HistorySectionTemplate Template { get; set; }

    public int Order { get; set; }

    public ICollection<HistorySectionContent> Contents { get; set; } = null!;
}
