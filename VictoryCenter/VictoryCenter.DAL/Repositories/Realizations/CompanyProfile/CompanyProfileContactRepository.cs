using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.CompanyProfile;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.CompanyProfile;

public class CompanyProfileContactRepository : RepositoryBase<CompanyProfileContact>, ICompanyProfileContactRepository
{
    public CompanyProfileContactRepository(VictoryCenterDbContext dbContext)
        : base(dbContext)
    {
    }
}
