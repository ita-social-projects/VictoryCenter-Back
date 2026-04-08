using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.MainPage;

public class MainPartnersRepository : RepositoryBase<MainPartners>, IMainPartnersRepository
{
    public MainPartnersRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
