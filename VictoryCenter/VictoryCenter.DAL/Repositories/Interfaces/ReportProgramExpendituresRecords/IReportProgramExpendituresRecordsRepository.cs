using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.DAL.Repositories.Interfaces.ReportProgramExpendituresRecords;

public interface IReportProgramExpendituresRecordsRepository : IRepositoryBase<ReportProgramExpendituresRecord>
{
    Task<bool> RecordWithinSameCategoryExistsAsync(ReportProgramExpendituresRecord record);

    Task<(decimal TotalAmountUah, decimal TotalAmountUsd)> GetSummaryAsync();
}
