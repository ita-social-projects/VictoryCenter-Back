using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.HistoryContents;

public class TitleHistoryContent : HistorySectionContent
{
    [Audited]
    public string Title { get; set; } = null!;
}
