using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Realizations.Base;
using MainPageEntity = VictoryCenter.DAL.Entities.MainPage;

namespace VictoryCenter.DAL.Repositories.Realizations.MainPage;

public class MainPageRepository : RepositoryBase<MainPageEntity>, IMainPageRepository
{
    public MainPageRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
