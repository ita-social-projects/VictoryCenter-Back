using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Donate;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Donate;

public class SupportOptionsRepository : RepositoryBase<SupportOptions>, ISupportOptionsRepository
{
    public SupportOptionsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
