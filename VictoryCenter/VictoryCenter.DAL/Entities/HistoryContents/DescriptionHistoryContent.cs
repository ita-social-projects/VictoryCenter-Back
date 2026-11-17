using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.HistoryContents;

public class DescriptionHistoryContent : HistorySectionContent
{
    [Audited]
    public string Description { get; set; } = null!;
}
