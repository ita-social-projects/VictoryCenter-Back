using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.VisitorPages;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.VisitorPages;

public class VisitorPagesRepository : RepositoryBase<VisitorPage>, IVisitorPagesRepository
{
    public VisitorPagesRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
