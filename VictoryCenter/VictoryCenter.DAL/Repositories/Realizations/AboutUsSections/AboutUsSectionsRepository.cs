using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.AboutUsSections;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.AboutUsSections;

public class AboutUsSectionsRepository : RepositoryBase<AboutUsSection>, IAboutUsSectionsRepository
{
    public AboutUsSectionsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
