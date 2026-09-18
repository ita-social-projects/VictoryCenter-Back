using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;

public abstract record BaseReportFundsExpendituresRecordDto
{
    public long CategoryId { get; init; }
    public decimal? Amount { get; init; }
    public ReportFundsExpendituresCurrency Currency { get; init; }
}
