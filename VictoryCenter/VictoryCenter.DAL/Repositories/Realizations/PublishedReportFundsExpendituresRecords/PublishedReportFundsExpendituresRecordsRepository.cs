using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.PublishedReportFundsExpendituresRecords;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.PublishedReportFundsExpendituresRecords;

public class PublishedReportFundsExpendituresRecordsRepository
    : RepositoryBase<PublishedReportFundsExpendituresRecord>, IPublishedReportFundsExpendituresRecordsRepository
{
    public PublishedReportFundsExpendituresRecordsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
