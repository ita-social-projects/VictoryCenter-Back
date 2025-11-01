using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.WhoWeAreSections;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.WhoWeAreSections;

public class WhoWeAreSectionsRepository : RepositoryBase<WhoWeAreSection>, IWhoWeAreSectionsRepository
{
    public WhoWeAreSectionsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
