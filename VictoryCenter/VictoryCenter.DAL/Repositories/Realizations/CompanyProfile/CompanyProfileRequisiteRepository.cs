using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.CompanyProfile;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.CompanyProfile;

public class CompanyProfileRequisiteRepository : RepositoryBase<CompanyProfileRequisite>, ICompanyProfileRequisiteRepository
{
    public CompanyProfileRequisiteRepository(VictoryCenterDbContext dbContext)
        : base(dbContext)
    {
    }
}
