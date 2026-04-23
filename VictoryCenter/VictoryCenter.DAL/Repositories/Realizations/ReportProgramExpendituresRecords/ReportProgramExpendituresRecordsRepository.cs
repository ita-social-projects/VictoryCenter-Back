using Microsoft.EntityFrameworkCore;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.ReportProgramExpendituresRecords;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.ReportProgramExpendituresRecords;

public class ReportProgramExpendituresRecordsRepository : RepositoryBase<ReportProgramExpendituresRecord>,
    IReportProgramExpendituresRecordsRepository
{
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
}
