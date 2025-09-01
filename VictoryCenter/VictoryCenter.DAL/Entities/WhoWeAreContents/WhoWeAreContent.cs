using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities.WhoWeAreContents;

public abstract class WhoWeAreContent
{
    public long Id { get; set; }

    public long SectionId { get; set; }

    public ContentType ContentType { get; set; }

    public WhoWeAreSection Section { get; set; }
}
