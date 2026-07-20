using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class PublishedReportFundsExpendituresRecord : BaseEntity
{
    public long SourceRecordId { get; set; }

    public string CategoryName { get; set; } = "";

    public string? CategoryNameEn { get; set; }

    public ReportFundsExpendituresType Type { get; set; }

    public int ReportingYear { get; set; }

    public decimal AmountUah { get; set; }

    public decimal AmountUsd { get; set; }
}
