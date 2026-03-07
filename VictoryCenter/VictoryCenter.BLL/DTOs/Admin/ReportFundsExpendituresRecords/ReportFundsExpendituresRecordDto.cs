using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;

public record ReportFundsExpendituresRecordDto
{
    public long Id { get; init; }
    public long CategoryId { get; init; }
    public ReportFundsExpendituresType Type { get; init; }
    public int ReportingYear { get; init; }
    public decimal AmountUah { get; init; }
    public decimal AmountUsd { get; init; }
}
