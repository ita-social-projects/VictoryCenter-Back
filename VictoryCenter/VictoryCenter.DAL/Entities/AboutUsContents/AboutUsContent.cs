using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities.AboutUsContents;

public class AboutUsContent
{
    public long Id { get; set; }

    public long SectionId { get; set; }

    public ContentType ContentType { get; set; }
}
