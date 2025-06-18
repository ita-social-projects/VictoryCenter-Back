using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.Utils.Seeder.CategoriesSeeder;

internal static class CategoriesDataSeeder
{
    private static readonly List<Category> _categories =
    [
        new()
        {
            Name = "Test name1",
            Description = "Test description1",
            CreatedAt = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),
        },
        new ()
        {
            Name = "Test name2",
            Description = "Test description2",
            CreatedAt = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),
        },
        new ()
        {
            Name = "Test name3",
            Description = "Test description3",
            CreatedAt = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),
        },
    ];

    public static void SeedData(VictoryCenterDbContext dbContext)
    {
        dbContext.Categories.AddRange(_categories);
        dbContext.SaveChanges();
    }
}
