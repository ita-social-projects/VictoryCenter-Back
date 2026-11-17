using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.HistoryContents;

public class ImageHistoryContent : HistorySectionContent
{
    [Audited]
    public long ImageId { get; set; }

    public Image Image { get; set; } = null!;
}
