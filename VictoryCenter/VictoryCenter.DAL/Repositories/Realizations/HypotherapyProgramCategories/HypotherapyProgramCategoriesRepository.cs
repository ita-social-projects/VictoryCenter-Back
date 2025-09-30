using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.HypotherapyProgramCategories;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.HypotherapyProgramCategories;

public class HypotherapyProgramCategoriesRepository : RepositoryBase<HypotherapyProgramCategory>, IHypotherapyProgramCategoriesRepository
{
    public HypotherapyProgramCategoriesRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
