using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.FaqPlacements;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.FaqPlacements;

public class FaqPlacementsRepository : RepositoryBase<FaqPlacement>, IFaqPlacementsRepository
{
    public FaqPlacementsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
