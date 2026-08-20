using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.HippotherapyLandingPages;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.HippotherapyLandingPages;

public class HippotherapyLandingPagesRepository : RepositoryBase<HippotherapyLandingPage>, IHippotherapyLandingPagesRepository
{
    public HippotherapyLandingPagesRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
