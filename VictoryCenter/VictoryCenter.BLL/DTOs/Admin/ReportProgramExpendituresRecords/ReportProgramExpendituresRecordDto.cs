namespace VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;

public record ReportProgramExpendituresRecordDto
{
    public long Id { get; init; }

    public int ReportingYear { get; init; }

    public long ProgramCategoryId { get; init; }

    public decimal AmountUah { get; set; }

    public decimal AmountUsd { get; set; }
}
