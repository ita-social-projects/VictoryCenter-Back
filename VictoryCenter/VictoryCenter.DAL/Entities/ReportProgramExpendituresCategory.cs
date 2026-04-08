using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class ReportProgramExpendituresCategory : BaseEntity
{
    public string Name { get; set; } = "";

    public ICollection<ReportProgramExpendituresRecord> ReportProgramExpendituresRecords { get; set; } = [];
}
