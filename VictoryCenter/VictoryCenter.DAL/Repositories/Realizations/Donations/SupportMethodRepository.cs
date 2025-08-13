using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Donations;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Donations;

public class SupportMethodRepository : RepositoryBase<SupportMethod>, ISupportMethodRepository
{
    public SupportMethodRepository(VictoryCenterDbContext context) : base(context)
    {
    }
}
