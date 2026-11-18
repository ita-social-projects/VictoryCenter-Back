using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.WhoWeAreContents;

public class TitleContent : WhoWeAreContent
{
    [Audited]
    [RichText]
    public string? Title { get; set; }
}
