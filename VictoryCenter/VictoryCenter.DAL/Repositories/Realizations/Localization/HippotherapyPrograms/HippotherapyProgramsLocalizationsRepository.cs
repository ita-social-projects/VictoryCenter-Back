using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.HippotherapyPrograms;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.HippotherapyPrograms;

public class HippotherapyProgramsLocalizationsRepository : RepositoryBase<HippotherapyProgramLocalization>, IHippotherapyProgramsLocalizationsRepository
{
    public HippotherapyProgramsLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
