using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.Utils.Seeder.CategoriesSeeder;

internal static class CategoriesDataSeeder
{
    private static List<Category> _categories =
    [
        new()
        {
            Name = "Test name1",
            Description = "Test description1",
            CreatedAt = DateTime.Now,
        },
        new ()
        {
            Name = "Test name2",
            Description = "Test description2",
            CreatedAt = DateTime.Now,
        },
        new ()
        {
            Name = "Test name3",
            Description = "Test description3",
            CreatedAt = DateTime.Now,
        },
    ];

    public static void SeedData(VictoryCenterDbContext dbContext)
        => dbContext.AddRange(_categories);
}
