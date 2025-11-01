namespace VictoryCenter.DAL.Entities.WhoWeAreContents;

public class ImageContent : WhoWeAreContent
{
    public long? ImageId { get; set; }

    public Image? Image { get; set; }
}
