using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities.AboutUsContents;

public abstract class AboutUsContent
{
    public long Id { get; set; }

    public long SectionId { get; set; }

    public ContentType ContentType { get; set; }

    public AboutUsSection Section { get; set; }
}
