using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.WhoWeAreContents;

public class CardContent : WhoWeAreContent
{
    [RichText]
    public string? Description { get; set; }
    public long? ImageId { get; set; }
    public Image? Image { get; set; }
}
