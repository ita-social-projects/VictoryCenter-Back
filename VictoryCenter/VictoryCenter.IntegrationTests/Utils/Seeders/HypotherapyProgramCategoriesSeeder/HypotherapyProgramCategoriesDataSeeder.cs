using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.HypotherapyProgramCategoriesSeeder;

public class ProgramCategoriesSeeder : BaseSeeder<ProgramCategory>
{
    public ProgramCategoriesSeeder(VictoryCenterDbContext dbContext, ILogger<ProgramCategoriesSeeder> logger)
        : base(dbContext, logger)
    {
    }

    public override string Name => "ProgramCategoriesSeeder";
    public override int Order => (int)SeederExecutionOrder.ProgramCategories;

    protected override Task<List<ProgramCategory>> GenerateEntitiesAsync()
    {
        var programCategories = new List<ProgramCategory>
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
