namespace VictoryCenter.DAL.Entities.AboutUsContents;

public class ImageContent : AboutUsContents.AboutUsContent
{
    public long? ImageId { get; set; }

    public Image? Image { get; set; }
}
