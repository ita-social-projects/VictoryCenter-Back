using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Donate;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Donate;

public class UahBankDetailsRepository : RepositoryBase<UahBankDetails>, IUahBankDetailsRepository
{
    public UahBankDetailsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
