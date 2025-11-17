using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class WhoWeAreSection
{
    public long Id { get; set; }

    public SectionType SectionType { get; set; }

    public required string Title { get; set; }

    public DateTime CreatedAt { get; set; }

    public required List<WhoWeAreContent> Contents { get; set; }
}
