using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using Microsoft.Extensions.Logging;
using VictoryCenter.BLL.Interfaces.BlobStorage;

namespace VictoryCenter.IntegrationTests.Utils.Seeder.ProgramCategoriesSeeder;

public class ProgramCategoriesSeeder : BaseSeeder<ProgramCategory>
{
    public ProgramCategoriesSeeder(VictoryCenterDbContext dbContext, ILogger<ProgramCategoriesSeeder> logger, IBlobService blobService)
        : base(dbContext, logger, blobService)
    {
    }

    public override string Name => "ProgramCategorySeeder";
    public override int Order => 3;
    protected override Task<bool> ShouldSkipAsync()
    {
        return Task.FromResult(_dbContext.ProgramCategories.Any());
    }

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
                Name = "TestName1",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 3,
                Name = "TestName1",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 4,
                Name = "TestName1",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 5,
                Name = "TestName1",
                CreatedAt = DateTime.UtcNow
            }
        };
        return Task.FromResult(programCategories);
    }
}
