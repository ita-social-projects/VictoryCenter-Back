using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.EventNewsCategorySeeder;

public class EventNewsCategoriesSeeder : BaseSeeder<EventNewsCategory>
{
    public EventNewsCategoriesSeeder(
        VictoryCenterDbContext dbContext,
        ILogger<EventNewsCategoriesSeeder> logger)
        : base(dbContext, logger)
    {
    }

    public override string Name => nameof(EventNewsCategoriesSeeder);
    public override int Order => (int)SeederExecutionOrder.EventNewsCategories;

    protected override Task<List<EventNewsCategory>> GenerateEntitiesAsync()
    {
        var categories = new List<EventNewsCategory>
        {
            new() { Id = 1, Name = "Новини", CreatedAt = DateTimeOffset.UtcNow },
            new() { Id = 2, Name = "Медіа", CreatedAt = DateTimeOffset.UtcNow },
            new() { Id = 3, Name = "Програми", CreatedAt = DateTimeOffset.UtcNow },
            new() { Id = 4, Name = "Події", CreatedAt = DateTimeOffset.UtcNow },
            new() { Id = 5, Name = "Репортажі", CreatedAt = DateTimeOffset.UtcNow },
        };

        return Task.FromResult(categories);
    }
}
