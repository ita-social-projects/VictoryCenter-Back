using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.CompanyProfile;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.CompanyProfile;

public class CompanyProfileContactLocalizationsRepository : RepositoryBase<CompanyProfileContactLocalization>, ICompanyProfileContactLocalizationsRepository
{
    public CompanyProfileContactLocalizationsRepository(VictoryCenterDbContext dbContext)
        : base(dbContext)
    {
    }
}
