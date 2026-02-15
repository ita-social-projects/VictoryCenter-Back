using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.HippotherapyProgramCategory;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.HippotherapyProgramCategory;

public class HippotherapyProgramCategoryLocalizationsRepository :RepositoryBase<HippotherapyProgramCategoryLocalization>, IHippotherapyProgramCategoryLocalizationsRepository
{
    public HippotherapyProgramCategoryLocalizationsRepository(VictoryCenterDbContext context) : base(context)
    {
    }
}
