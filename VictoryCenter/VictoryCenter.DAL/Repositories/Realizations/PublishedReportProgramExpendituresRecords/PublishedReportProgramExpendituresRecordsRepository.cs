using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.PublishedReportProgramExpendituresRecords;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.PublishedReportProgramExpendituresRecords;

public class PublishedReportProgramExpendituresRecordsRepository
    : RepositoryBase<PublishedReportProgramExpendituresRecord>, IPublishedReportProgramExpendituresRecordsRepository
{
    public PublishedReportProgramExpendituresRecordsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
