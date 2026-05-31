using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.ReportProgramExpendituresRecords;

public class ReportProgramExpendituresRecordsRepository : RepositoryBase<ReportProgramExpendituresRecord>,
    IReportProgramExpendituresRecordsRepository
{
    private readonly record struct Summary(decimal TotalAmountUah, decimal TotalAmountUsd);

    public ReportProgramExpendituresRecordsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }

    public async Task<bool> RecordWithinSameCategoryWithSameYearExistsAsync(ReportProgramExpendituresRecord record)
    {
        return await DbContext
            .ReportProgramExpendituresRecords
            .AnyAsync(e =>
                e.HippotherapyProgramCategoryId == record.HippotherapyProgramCategoryId &&
                e.ReportingYear == record.ReportingYear);
    }

    public async Task<(decimal TotalAmountUah, decimal TotalAmountUsd)> GetSummaryAsync()
    {
        var summary = await DbContext
            .ReportProgramExpendituresRecords
            .AsNoTracking()
            .GroupBy(_ => 1)
            .Select(group => new Summary(
                group.Sum(record => record.AmountUah),
                group.Sum(record => record.AmountUsd)))
            .FirstOrDefaultAsync();

        return (
            Math.Round(summary.TotalAmountUah, 2, MidpointRounding.AwayFromZero),
            Math.Round(summary.TotalAmountUsd, 2, MidpointRounding.AwayFromZero));
    }
}
