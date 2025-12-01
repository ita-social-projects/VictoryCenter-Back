using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Interfaces.WhoWeAreContents;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.WhoWeAreContents;

public class WhoWeAreContentsRepository : RepositoryBase<WhoWeAreContent>, IWhoWeAreContentsRepository
{
    public WhoWeAreContentsRepository(VictoryCenterDbContext dbContext)
        : base(dbContext)
    {
    }
}
