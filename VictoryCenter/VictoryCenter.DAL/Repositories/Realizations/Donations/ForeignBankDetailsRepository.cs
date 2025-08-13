using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Donations;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Donations;

public class ForeignBankDetailsRepository : RepositoryBase<ForeignBankDetails>, IForeignBankDetailsRepository
{
    public ForeignBankDetailsRepository(VictoryCenterDbContext context) : base(context)
    {
    }
}
