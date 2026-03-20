namespace VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;

public record ReportFundsExpendituresSummaryDto
{
    public decimal IncomeUahTotal { get; init; }

    public decimal IncomeUsdTotal { get; init; }

    public decimal ExpenditureUahTotal { get; init; }

    public decimal ExpenditureUsdTotal { get; init; }

    public int IncomeCategoriesCount { get; init; }

    public int ExpenditureCategoriesCount { get; init; }
}
