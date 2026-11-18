using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class WhoWeAreSection
{
    public long Id { get; set; }

    [Audited]
    public SectionType SectionType { get; set; }

    [Audited]
    public required string Title { get; set; }

    public DateTime CreatedAt { get; set; }

    public required List<WhoWeAreContent> Contents { get; set; }
}
