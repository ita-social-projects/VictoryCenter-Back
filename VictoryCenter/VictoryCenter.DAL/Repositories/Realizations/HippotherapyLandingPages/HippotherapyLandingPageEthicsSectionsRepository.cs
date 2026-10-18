using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.HippotherapyLandingPageContents;
using VictoryCenter.DAL.Repositories.Interfaces.HippotherapyLandingPages;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.HippotherapyLandingPages;

public class HippotherapyLandingPageEthicsSectionsRepository
    : RepositoryBase<HippotherapyLandingPageEthicsSection>, IHippotherapyLandingPageEthicsSectionsRepository
{
    public HippotherapyLandingPageEthicsSectionsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
