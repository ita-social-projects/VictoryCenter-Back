using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.MainPage;

public class MainAboutUsRepository : RepositoryBase<MainAboutUs>, IMainAboutUsRepository
{
    public MainAboutUsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
