using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.HippotherapyPrograms;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.HippotherapyPrograms;

public class HippotherapyProgramsRepository : RepositoryBase<HippotherapyProgram>, IHippotherapyProgramsRepository
{
    public HippotherapyProgramsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
