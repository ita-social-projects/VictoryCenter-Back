using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.WhoWeAreContents;

public class TitleContent : WhoWeAreContent
{
    [RichText]
    public string? Title { get; set; }
}
