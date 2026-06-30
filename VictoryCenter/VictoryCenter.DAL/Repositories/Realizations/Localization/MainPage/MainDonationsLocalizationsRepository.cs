using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.MainPage;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.MainPage;

public class MainDonationsLocalizationsRepository
    : RepositoryBase<MainDonationsLocalization>, IMainDonationsLocalizationsRepository
{
    public MainDonationsLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
