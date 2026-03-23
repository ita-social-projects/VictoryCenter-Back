using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.CompanyProfile;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.CompanyProfile;

public class CompanyProfileRequisiteLocalizationsRepository : RepositoryBase<CompanyProfileRequisiteLocalization>, ICompanyProfileRequisiteLocalizationsRepository
{
    public CompanyProfileRequisiteLocalizationsRepository(VictoryCenterDbContext dbContext)
        : base(dbContext)
    {
    }
}
