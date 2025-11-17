using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Donate;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Donate;

public class CorrespondentBankDetailsRepository : RepositoryBase<CorrespondentBankDetails>, ICorrespondentBankDetailsRepository
{
    public CorrespondentBankDetailsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
