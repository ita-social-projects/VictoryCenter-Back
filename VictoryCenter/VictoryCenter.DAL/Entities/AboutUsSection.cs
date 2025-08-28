using VictoryCenter.DAL.Entities.AboutUsContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class AboutUsSection
{
    public long Id { get; set; }

    public SectionType SectionType { get; set; }

    public required string Title { get; set; }

    public DateTime CreatedAt { get; set; }

    public required List<AboutUsContent> Contents { get; set; }
}
