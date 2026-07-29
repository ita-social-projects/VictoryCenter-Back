using VictoryCenter.DAL.Repositories.Interfaces.Base;
using PublishedSnapshotEntity = VictoryCenter.DAL.Entities.PublishedReportFundsExpendituresSnapshot;

namespace VictoryCenter.DAL.Repositories.Interfaces.PublishedReportFundsExpendituresSnapshot;

public interface IPublishedReportFundsExpendituresSnapshotRepository
    : IRepositoryBase<PublishedSnapshotEntity>
{
}
