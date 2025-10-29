using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.HippotherapyProgramCategoriesSeeder;

public class HippotherapyProgramCategoriesSeeder : BaseSeeder<HippotherapyProgramCategory>
{
    public HippotherapyProgramCategoriesSeeder(VictoryCenterDbContext dbContext, ILogger<HippotherapyProgramCategoriesSeeder> logger)
        : base(dbContext, logger)
    {
    }

    public override string Name => "HippotherapyProgramCategoriesSeeder";
    public override int Order => (int)SeederExecutionOrder.HippotherapyProgramCategories;

    protected override Task<List<HippotherapyProgramCategory>> GenerateEntitiesAsync()
    {
        var programCategories = new List<HippotherapyProgramCategory>
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
