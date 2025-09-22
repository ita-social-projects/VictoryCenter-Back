using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Programs;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Programs;

public class ProgramsRepository : RepositoryBase<Program>, IProgramsRepository
{
    public ProgramsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
