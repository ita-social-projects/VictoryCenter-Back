using VictoryCenter.DAL.Data.BaseEntity;

namespace VictoryCenter.DAL.Entities;

public class PublishedReportProgramExpendituresRecord : BaseEntity
{
    public long SourceRecordId { get; set; }

    public string CategoryName { get; set; } = "";

    public string? CategoryNameEn { get; set; }

    public int ReportingYear { get; set; }

    public decimal AmountUah { get; set; }

    public decimal AmountUsd { get; set; }
}
