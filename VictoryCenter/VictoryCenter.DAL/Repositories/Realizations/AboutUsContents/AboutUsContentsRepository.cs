using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.AboutUsContents;
using VictoryCenter.DAL.Repositories.Interfaces.AboutUsContents;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.AboutUsContents;

public class AboutUsContentsRepository : RepositoryBase<AboutUsContent>, IAboutUsContentsRepository
{
    public AboutUsContentsRepository(VictoryCenterDbContext dbContext)
        : base(dbContext)
    {
    }
}
