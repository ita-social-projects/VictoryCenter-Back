using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.WhoWeAreContents;

public class DescriptionContent : WhoWeAreContent
{
    [RichText]
    public string? Description { get; set; }
}
