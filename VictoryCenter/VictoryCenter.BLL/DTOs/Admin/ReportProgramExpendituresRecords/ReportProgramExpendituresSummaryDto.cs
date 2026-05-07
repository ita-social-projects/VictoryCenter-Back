namespace VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;

public record ReportProgramExpendituresSummaryDto
{
    public decimal TotalAmountUah { get; init; }

    public decimal TotalAmountUsd { get; init; }
}
