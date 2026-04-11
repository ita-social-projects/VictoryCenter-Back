namespace VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;

public record CreateReportProgramExpendituresRecordDto
{
    public int ReportingYear { get; init; }

    public long HippotherapyProgramCategoryId { get; init; }

    public decimal AmountUah { get; set; }

    public decimal AmountUsd { get; set; }
}
