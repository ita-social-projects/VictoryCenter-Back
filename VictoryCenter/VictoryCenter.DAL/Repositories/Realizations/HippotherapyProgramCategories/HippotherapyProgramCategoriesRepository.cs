using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.HippotherapyProgramCategories;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.HippotherapyProgramCategories;

public class HippotherapyProgramCategoriesRepository : RepositoryBase<HippotherapyProgramCategory>, IHippotherapyProgramCategoriesRepository
{
    public HippotherapyProgramCategoriesRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
