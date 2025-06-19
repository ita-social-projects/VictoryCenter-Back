using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.Utils.Seeder.CategoriesSeeder;
using VictoryCenter.IntegrationTests.Utils.Seeder.TeamMembersSeeder;
using VictoryCenter.IntegrationTests.Utils.Seeder.TestSeeder;

namespace VictoryCenter.IntegrationTests.Utils.Seeder;

internal static class IntegrationTestsDatabaseSeeder
{
    public static void SeedData(VictoryCenterDbContext dbContext)
    {
        TeamMembersDataSeeder.SeedData(dbContext);
        TestDataSeeder.SeedData(dbContext);
        CategoriesDataSeeder.SeedData(dbContext);
        dbContext.SaveChanges();
    }

    public static async Task DeleteExistingData(VictoryCenterDbContext dbContext)
    {
        dbContext.TeamMembers.RemoveRange(dbContext.TeamMembers);
        dbContext.SaveChanges();
        await Task.Delay(100); // Ensure the deletion is processed
        dbContext.Categories.RemoveRange(dbContext.Categories);
        dbContext.TestEntities.RemoveRange(dbContext.TestEntities);
        dbContext.SaveChanges();
    }
}
