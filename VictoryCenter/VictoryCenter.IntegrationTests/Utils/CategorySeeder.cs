using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.Utils;

internal static class CategorySeeder
{
    private static readonly List<Category> _testCategories =
    [
        new () {Name = "Category 1", Description = "Category 1 Decription", CreatedAt = DateTime.Now.AddDays(-10)},
        new () {Name = "Category 2", Description = "Category 2 Decription", CreatedAt = DateTime.Now.AddDays(-20)},
        new () {Name = "Category 3", Description = "Category 3 Decription", CreatedAt = DateTime.Now.AddDays(-30)},
    ];

    public static void SeedCategories(VictoryCenterDbContext dbContext)
    {
        dbContext.AddRange(_testCategories);
        dbContext.SaveChanges();
    }

    public static void DeleteExistingCategories(VictoryCenterDbContext dbContext)
    {
        dbContext.Categories.RemoveRange(dbContext.Categories);
        dbContext.SaveChanges();
    }
}
