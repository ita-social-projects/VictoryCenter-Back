using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Realizations.Base;
using VictoryCenter.DAL.Repositories.Interfaces.ProgramCategories;

namespace VictoryCenter.DAL.Repositories.Realizations.ProgramCategories;

public class ProgramCategoriesRepository : RepositoryBase<ProgramCategory>, IProgramCategoriesRepository
{
    public ProgramCategoriesRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
