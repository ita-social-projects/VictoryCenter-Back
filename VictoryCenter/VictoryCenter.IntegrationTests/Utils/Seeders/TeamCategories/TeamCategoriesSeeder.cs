using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.TeamCategories;

public class TeamCategoriesSeeder : BaseSeeder<TeamCategory>
{
    public TeamCategoriesSeeder(VictoryCenterDbContext dbContext, ILogger<TeamCategoriesSeeder> logger)
        : base(dbContext, logger)
    {
    }

    public override string Name => nameof(TeamCategoriesSeeder);
    public override int Order => (int)SeederExecutionOrder.Categories;

    protected override Task<List<TeamCategory>> GenerateEntitiesAsync()
    {
        var categories = new List<TeamCategory>
        {
            new()
            {
                Name = "Test name1",
                Description = "Test description1",
                CreatedAt = DateTimeOffset.UtcNow
            },
            new ()
            {
                Name = "Test name2",
                Description = "Test description2",
                CreatedAt = DateTimeOffset.UtcNow
            },
            new ()
            {
                Name = "Test name3",
                Description = "Test description3",
                CreatedAt = DateTimeOffset.UtcNow,
            },
            new ()
            {
                Name = "Test name4",
                Description = "Test description4",
                CreatedAt = DateTimeOffset.UtcNow,
                TeamMembers = []
            }
        };

        return Task.FromResult(categories);
    }
}
