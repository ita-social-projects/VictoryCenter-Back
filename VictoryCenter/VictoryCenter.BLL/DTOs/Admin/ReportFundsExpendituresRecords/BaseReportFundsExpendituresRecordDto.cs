namespace VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;

public abstract record BaseReportFundsExpendituresRecordDto
{
    public long CategoryId { get; init; }
    public decimal AmountUah { get; init; }
    public decimal AmountUsd { get; init; }
}
