using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;

public record CreateReportFundsExpendituresRecordDto : BaseReportFundsExpendituresRecordDto
{
    public ReportFundsExpendituresType Type { get; init; }
    public int ReportingYear { get; init; }
}
