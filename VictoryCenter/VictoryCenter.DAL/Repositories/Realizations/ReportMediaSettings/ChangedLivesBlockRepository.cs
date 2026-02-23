using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.ReportMediaSettings;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.ReportMediaSettings;

public class ChangedLivesBlockRepository : RepositoryBase<ChangedLivesBlock>, IChangedLivesBlockRepository
{
    public ChangedLivesBlockRepository(VictoryCenterDbContext context)
        : base(context)
    {
    }
}
