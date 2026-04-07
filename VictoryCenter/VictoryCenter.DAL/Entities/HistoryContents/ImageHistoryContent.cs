namespace VictoryCenter.DAL.Entities.HistoryContents;

public class ImageHistoryContent : HistorySectionContent
{
    public long ImageId { get; set; }

    public Image Image { get; set; } = null!;
}
