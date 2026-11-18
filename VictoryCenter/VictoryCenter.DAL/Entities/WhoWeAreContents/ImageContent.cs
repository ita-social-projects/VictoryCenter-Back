using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.WhoWeAreContents;

public class ImageContent : WhoWeAreContent
{
    [Audited]
    public long? ImageId { get; set; }

    public Image? Image { get; set; }
}
