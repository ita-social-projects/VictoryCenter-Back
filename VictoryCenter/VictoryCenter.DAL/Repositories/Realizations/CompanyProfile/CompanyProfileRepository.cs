using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Repositories.Interfaces.CompanyProfile;
using VictoryCenter.DAL.Repositories.Realizations.Base;
using CompanyProfileEntity = VictoryCenter.DAL.Entities.CompanyProfile;

namespace VictoryCenter.DAL.Repositories.Realizations.CompanyProfile;

public class CompanyProfileRepository : RepositoryBase<CompanyProfileEntity>, ICompanyProfileRepository
{
    public CompanyProfileRepository(VictoryCenterDbContext dbContext)
        : base(dbContext)
    {
    }
}
