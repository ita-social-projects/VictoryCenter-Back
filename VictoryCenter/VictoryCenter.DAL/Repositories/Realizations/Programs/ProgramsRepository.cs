using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Realizations.Base;
using VictoryCenter.DAL.Repositories.Interfaces.Programs;

namespace VictoryCenter.DAL.Repositories.Realizations.Programs;

public class ProgramsRepository : RepositoryBase<Program>, IProgramsRepository
{
    public ProgramsRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
