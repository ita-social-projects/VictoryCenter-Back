using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Donate;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Donate;

public class ForeignBankDetailsRepository : RepositoryBase<ForeignBankDetails>, IForeignBankDetailsRepository
{
    public ForeignBankDetailsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
