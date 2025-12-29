using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class HippotherapyProgramSection : BaseEntity
{
    public long ProgramId { get; set; }

    public ProgramSectionTemplate Template { get; set; }

    public int Order { get; set; }

    public HippotherapyProgram Program { get; set; } = null!;

    public ICollection<ProgramSectionContent> Contents { get; set; } = null!;
}
