using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class BackupReportProgramExpendituresRecord : BaseEntity
{
    public int ReportingYear { get; set; }
    public long HippotherapyProgramCategoryId { get; set; }
    public decimal AmountUah { get; set; }
    public decimal AmountUsd { get; set; }

    public HippotherapyProgramCategory HippotherapyProgramCategory { get; set; } = null!;
}
