using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.MainPage;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.MainPage;

public class MainDonationsRepository : RepositoryBase<MainDonations>, IMainDonationsRepository
{
    public MainDonationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
