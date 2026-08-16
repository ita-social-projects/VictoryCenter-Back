using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.HippotherapyProgramCategories;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.HippotherapyProgramCategories;

public class HippotherapyProgramCategoryLocalizationsRepository
    : RepositoryBase<HippotherapyProgramCategoryLocalization>,
      IHippotherapyProgramCategoryLocalizationsRepository
{
    public HippotherapyProgramCategoryLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
