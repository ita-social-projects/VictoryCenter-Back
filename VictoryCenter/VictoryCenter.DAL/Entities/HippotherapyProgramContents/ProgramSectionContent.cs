using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities.HippotherapyProgramContents;

public abstract class ProgramSectionContent
{
    public long Id { get; set; }

    public long SectionId { get; set; }

    public ContentType ContentType { get; set; }

    public int Order { get; set; }

    public HippotherapyProgramSection Section { get; set; } = null!;
}
