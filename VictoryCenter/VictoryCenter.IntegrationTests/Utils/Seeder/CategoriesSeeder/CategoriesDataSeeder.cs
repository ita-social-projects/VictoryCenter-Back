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
            CreatedAt = new DateTime(2025, 2, 10),
        },
        new ()
        {
            Name = "Test name2",
            Description = "Test description2",
            CreatedAt = new DateTime(2025, 2, 10),
        },
        new ()
        {
            Name = "Test name3",
            Description = "Test description3",
            CreatedAt = new DateTime(2025, 2, 10),
        },
        new ()
        {
            Name = "TestForDelete",
            Description = "Test description",
            CreatedAt = new DateTime(2025, 12, 10)
        },
        new ()
        {
            Name = "CreateMember",
            Description = "Super Description",
            CreatedAt = new DateTime(2025, 2, 2)
        },
    ];

    public static void SeedData(VictoryCenterDbContext dbContext)
        => dbContext.AddRange(_categories);
}
