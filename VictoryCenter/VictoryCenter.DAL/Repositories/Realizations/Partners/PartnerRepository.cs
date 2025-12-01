using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Partners;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Partners;

public class PartnerRepository : RepositoryBase<Partner>, IPartnerRepository
{
    public PartnerRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
