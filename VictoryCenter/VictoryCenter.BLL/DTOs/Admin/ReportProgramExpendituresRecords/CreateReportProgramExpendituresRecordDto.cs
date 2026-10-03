namespace VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;

public record CreateReportProgramExpendituresRecordDto : BaseReportProgramExpendituresRecordDto
{
    public int ReportingYear { get; init; }
}
