namespace VictoryCenter.DAL.Entities.AboutUsContents;

public class CardContent : AboutUsContent
{
    public string? Description { get; set; }
    public long? ImageId { get; set; }
    public Image? Image { get; set; }
}
