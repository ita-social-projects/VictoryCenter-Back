using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class HippotherapyProgramCategory : BaseEntity
{
    public string Name { get; set; } = null!;
    public ICollection<HippotherapyProgram> Programs { get; set; } = new List<HippotherapyProgram>();

    public ICollection<ReportProgramExpendituresRecord> ReportProgramExpendituresRecords { get; set; } =
        new List<ReportProgramExpendituresRecord>();
}
