using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using Microsoft.Extensions.Logging;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.ProgramCategoriesSeeder;

public class ProgramCategoriesSeeder : BaseSeeder<ProgramCategory>
{
    public ProgramCategoriesSeeder(VictoryCenterDbContext dbContext, ILogger<ProgramCategoriesSeeder> logger)
        : base(dbContext, logger)
    {
    }

    public override string Name => "ProgramCategoriesSeeder";
    public override int Order => 3;

    protected override Task<List<ProgramCategory>> GenerateEntitiesAsync()
    {
        var programCategories = new List<ProgramCategory>
        {
            new()
            {
                Id = 1,
                Name = "TestName1",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 2,
                Name = "TestName2",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 3,
                Name = "TestName3",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 4,
                Name = "TestName4",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 5,
                Name = "TestName5",
                CreatedAt = DateTime.UtcNow
            }
        };
        return Task.FromResult(programCategories);
    }
}
