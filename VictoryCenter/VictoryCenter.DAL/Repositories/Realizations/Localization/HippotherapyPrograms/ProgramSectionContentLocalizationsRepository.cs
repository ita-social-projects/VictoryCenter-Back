using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Repositories.Interfaces.Localization.HippotherapyPrograms;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Localization.HippotherapyPrograms;

public class ProgramSectionContentLocalizationsRepository : RepositoryBase<ProgramSectionContentLocalization>, IProgramSectionContentLocalizationsRepository
{
    public ProgramSectionContentLocalizationsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
