namespace VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;

public record UpdateReportProgramExpendituresRecordDto
{
    public long HippotherapyProgramCategoryId { get; init; }

    public decimal AmountUah { get; init; }

    public decimal AmountUsd { get; init; }
}
