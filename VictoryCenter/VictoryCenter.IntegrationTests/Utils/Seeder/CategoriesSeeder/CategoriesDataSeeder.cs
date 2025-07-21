using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.Utils.Seeder.Seeders;

public class CategoriesSeeder : BaseSeeder<Category>
{
    public CategoriesSeeder(VictoryCenterDbContext dbContext, ILogger<CategoriesSeeder> logger)
        : base(dbContext, logger)
    {
    }

    public override string Name => "CategoriesSeeder";
    public override int Order => 1;

    protected override Task<bool> ShouldSkipAsync()
    {
        return Task.FromResult(_dbContext.Categories.Any());
    }

    protected override Task<List<Category>> GenerateEntitiesAsync()
    {
        var categories = new List<Category>
        {
            new()
            {
                Name = "Test name1",
                Description = "Test description1",
                CreatedAt = DateTime.UtcNow
            },
            new ()
            {
                Name = "Test name2",
                Description = "Test description2",
                CreatedAt = DateTime.UtcNow
            },
            new ()
            {
                Name = "Test name3",
                Description = "Test description3",
                CreatedAt = DateTime.UtcNow
            }
        };

        return Task.FromResult(categories);
    }
}
