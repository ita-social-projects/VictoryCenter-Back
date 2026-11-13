using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.WhoWeAreContents;

public class CardContent : WhoWeAreContent
{
    [Audited]
    [RichText]
    public string? Description { get; set; }

    [Audited]
    public long? ImageId { get; set; }

    public Image? Image { get; set; }
}
