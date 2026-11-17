using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class HippotherapyProgramSection : BaseEntity
{
    [Audited]
    public long ProgramId { get; set; }

    [Audited]
    public ProgramSectionTemplate Template { get; set; }

    [Audited]
    public int Order { get; set; }

    public HippotherapyProgram Program { get; set; } = null!;

    public ICollection<ProgramSectionContent> Contents { get; set; } = null!;
}
