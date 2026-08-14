using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.PublishedReportFundsExpendituresSnapshot;
using VictoryCenter.DAL.Repositories.Realizations.Base;
using PublishedSnapshotEntity = VictoryCenter.DAL.Entities.PublishedReportFundsExpendituresSnapshot;

namespace VictoryCenter.DAL.Repositories.Realizations.PublishedReportFundsExpendituresSnapshot;

public class PublishedReportFundsExpendituresSnapshotRepository
    : RepositoryBase<PublishedSnapshotEntity>, IPublishedReportFundsExpendituresSnapshotRepository
{
    public PublishedReportFundsExpendituresSnapshotRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
