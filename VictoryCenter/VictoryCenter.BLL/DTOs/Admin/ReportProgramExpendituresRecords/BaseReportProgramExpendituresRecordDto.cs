namespace VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;

public abstract record BaseReportProgramExpendituresRecordDto
{
    public long HippotherapyProgramCategoryId { get; init; }

    public decimal? AmountUah { get; init; }

    public decimal? AmountUsd { get; init; }
}
