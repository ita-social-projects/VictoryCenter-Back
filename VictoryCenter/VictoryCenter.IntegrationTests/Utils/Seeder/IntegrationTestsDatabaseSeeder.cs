using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.Utils.Seeder.CategoriesSeeder;
using VictoryCenter.IntegrationTests.Utils.Seeder.TestSeeder;

namespace VictoryCenter.IntegrationTests.Utils.Seeder;

internal static class IntegrationTestsDatabaseSeeder
{
    public static void SeedData(VictoryCenterDbContext dbContext)
    {
        TestDataSeeder.SeedData(dbContext);
        CategoriesDataSeeder.SeedData(dbContext);
        dbContext.SaveChanges();
    }

    public static void DeleteExistingData(VictoryCenterDbContext dbContext)
    {
        dbContext.TestEntities.RemoveRange(dbContext.TestEntities);
        dbContext.Categories.RemoveRange(dbContext.Categories);
        dbContext.SaveChanges();
    }
}
