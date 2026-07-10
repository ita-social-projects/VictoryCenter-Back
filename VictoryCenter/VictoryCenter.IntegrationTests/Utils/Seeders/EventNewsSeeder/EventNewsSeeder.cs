using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Slugify;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Enums;
using EventNewsEntity = VictoryCenter.DAL.Entities.EventNews;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.EventNewsSeeder;

public class EventNewsSeeder : BaseSeeder<EventNewsEntity>
{
    private const int EventNewsCount = 8;
    private readonly SlugHelper _slugHelper = new();

    public EventNewsSeeder(VictoryCenterDbContext dbContext, ILogger<EventNewsSeeder> logger)
        : base(dbContext, logger)
    {
    }

    public override string Name => nameof(EventNewsSeeder);
    public override int Order => (int)SeederExecutionOrder.EventNews;

    protected override async Task<List<EventNewsEntity>> GenerateEntitiesAsync()
    {
        var categories = await DbContext.EventNewsCategories.Take(4).ToListAsync();

        var eventNewsItems = new List<EventNewsEntity>();

        for (var i = 0; i < EventNewsCount; i++)
        {
            var selectedCategories = categories
                .OrderBy(_ => Guid.NewGuid())
                .Take(2)
                .ToList();

            var title = "TestEventNews" + (i + 1);

            eventNewsItems.Add(new EventNewsEntity
            {
                Id = i + 1,
                Slug = _slugHelper.GenerateSlug(title),
                Resource = "TestResource" + (i + 1),
                PublishedAt = DateTimeOffset.UtcNow.AddDays(-i),
                Status = (Status)(i % Enum.GetNames<Status>().Length),
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-i),
                Categories = selectedCategories,
            });
        }

        return eventNewsItems;
    }
}
