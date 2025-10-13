using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.HypotherapyPrograms;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.HypotherapyPrograms;

public class HypotherapyProgramsRepository : RepositoryBase<HippotherapyProgram>, IHypotherapyProgramsRepository
{
    public HypotherapyProgramsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
