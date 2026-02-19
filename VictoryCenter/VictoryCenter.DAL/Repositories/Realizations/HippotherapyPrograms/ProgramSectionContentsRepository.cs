using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.HippotherapyProgramContents;
using VictoryCenter.DAL.Repositories.Interfaces.HippotherapyPrograms;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.HippotherapyPrograms;

public class ProgramSectionContentsRepository : RepositoryBase<ProgramSectionContent>, IProgramSectionContentsRepository
{
    public ProgramSectionContentsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
