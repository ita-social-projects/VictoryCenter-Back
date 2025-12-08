using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Partners;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Partners;

public class PartnerSectionsRepository : RepositoryBase<PartnerSection>, IPartnerSectionsRepository
{
    public PartnerSectionsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
