using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class ReportProgramExpendituresRecord : BaseEntity
{
    public int ReportingYear { get; init; }

    public long ProgramCategoryId { get; set; }

    public decimal AmountUah { get; set; }

    public decimal AmountUsd { get; set; }

    public ReportProgramExpendituresCategory ProgramCategory { get; set; } = null!;
}
