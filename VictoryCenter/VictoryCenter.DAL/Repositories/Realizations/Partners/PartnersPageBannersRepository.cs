using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Partners;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Partners;

public class PartnersPageBannersRepository : RepositoryBase<PartnersPageBanner>, IPartnersPageBannersRepository
{
    public PartnersPageBannersRepository(VictoryCenterDbContext context)
    : base(context)
    {
    }
}
