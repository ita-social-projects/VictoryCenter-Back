using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Donations;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Donations;

public class UahBankDetailsRepository : RepositoryBase<UahBankDetails>, IUahBankDetailsRepository
{
    public UahBankDetailsRepository(VictoryCenterDbContext context) : base(context)
    {
    }
}
