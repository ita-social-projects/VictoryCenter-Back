using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.DAL.Repositories.Interfaces.ReportFundsExpendituresRecords;

public interface IReportFundsExpendituresRecordsRepository : IRepositoryBase<ReportFundsExpendituresRecord>
{
    Task<(decimal IncomeUahTotal, decimal IncomeUsdTotal, int IncomeCategoriesCount,
        decimal ExpenditureUahTotal, decimal ExpenditureUsdTotal, int ExpenditureCategoriesCount)>
        GetSummaryAsync();
}
