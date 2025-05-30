using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Repositories.Interfaces.Test;
using VictoryCenter.DAL.Repositories.Realizations.Base;

namespace VictoryCenter.DAL.Repositories.Realizations.Test;

public class TestRepository : RepositoryBase<TestEntity>, ITestRepository
{
    public TestRepository(VictoryCenterDbContext context) : base(context)
    {
    }
}
