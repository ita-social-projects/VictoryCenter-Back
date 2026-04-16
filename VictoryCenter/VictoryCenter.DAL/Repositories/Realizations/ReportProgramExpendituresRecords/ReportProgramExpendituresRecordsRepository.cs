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
}
