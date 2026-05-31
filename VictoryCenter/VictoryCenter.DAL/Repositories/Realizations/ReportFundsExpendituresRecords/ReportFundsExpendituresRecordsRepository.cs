using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresRecords;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.ReportFundsExpendituresRecords;

public class ReportFundsExpendituresRecordsRepository
    : RepositoryBase<ReportFundsExpendituresRecord>, IReportFundsExpendituresRecordsRepository
{
    private readonly record struct TypeSummary(decimal UahTotal, decimal UsdTotal, int CategoriesCount);

    public ReportFundsExpendituresRecordsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }

    public async Task<(decimal IncomeUahTotal, decimal IncomeUsdTotal, int IncomeCategoriesCount,
        decimal ExpenditureUahTotal, decimal ExpenditureUsdTotal, int ExpenditureCategoriesCount)>
        GetSummaryAsync()
    {
        var summaryByType = await BuildSummaryByTypeAsync();
        var incomeSummary = summaryByType.GetValueOrDefault(ReportFundsExpendituresType.Income);
        var expenditureSummary = summaryByType.GetValueOrDefault(ReportFundsExpendituresType.Expense);

        return (
            incomeSummary.UahTotal,
            incomeSummary.UsdTotal,
            incomeSummary.CategoriesCount,
            expenditureSummary.UahTotal,
            expenditureSummary.UsdTotal,
            expenditureSummary.CategoriesCount);
    }

    private async Task<Dictionary<ReportFundsExpendituresType, TypeSummary>> BuildSummaryByTypeAsync()
    {
        var rawSummaries = await DbContext.ReportFundsExpendituresRecords
            .AsNoTracking()
            .GroupBy(record => record.Type)
            .Select(group => new
            {
                Type = group.Key,
                UahTotal = group.Sum(record => record.AmountUah),
                UsdTotal = group.Sum(record => record.AmountUsd),
                CategoriesCount = group.Select(record => record.CategoryId).Distinct().Count(),
            })
            .ToListAsync();

        return rawSummaries.ToDictionary(
            summary => summary.Type,
            summary => new TypeSummary(
                Math.Round(summary.UahTotal, 0, MidpointRounding.AwayFromZero),
                Math.Round(summary.UsdTotal, 0, MidpointRounding.AwayFromZero),
                summary.CategoriesCount));
    }
}
