using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.HypotherapyProgramCategoriesSeeder;

public class HypotherapyProgramCategoriesSeeder : BaseSeeder<HypotherapyProgramCategory>
{
    public HypotherapyProgramCategoriesSeeder(VictoryCenterDbContext dbContext, ILogger<HypotherapyProgramCategoriesSeeder> logger)
        : base(dbContext, logger)
    {
    }

    public override string Name => "ProgramCategoriesSeeder";
    public override int Order => (int)SeederExecutionOrder.ProgramCategories;

    protected override Task<List<HypotherapyProgramCategory>> GenerateEntitiesAsync()
    {
        var programCategories = new List<HypotherapyProgramCategory>
        {
            new()
            {
                Id = 1,
                Name = "TestName1",
                CreatedAt = DateTimeOffset.UtcNow
            },
            new()
            {
                Id = 2,
                Name = "TestName2",
                CreatedAt = DateTimeOffset.UtcNow
            },
            new()
            {
                Id = 3,
                Name = "TestName3",
                CreatedAt = DateTimeOffset.UtcNow
            },
            new()
            {
                Id = 4,
                Name = "TestName4",
                CreatedAt = DateTimeOffset.UtcNow
            },
            new()
            {
                Id = 5,
                Name = "TestName5",
                CreatedAt = DateTimeOffset.UtcNow
            }
        };
        return Task.FromResult(programCategories);
    }
}
