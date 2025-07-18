using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Pages;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Pages;

public class PagesRepository : RepositoryBase<Page>, IPagesRepository
{
    public PagesRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
