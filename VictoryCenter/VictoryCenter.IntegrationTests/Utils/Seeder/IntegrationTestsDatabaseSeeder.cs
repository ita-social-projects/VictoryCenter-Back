using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.Utils.Seeder.CategoriesSeeder;
using VictoryCenter.IntegrationTests.Utils.Seeder.TeamMembersSeeder;
using VictoryCenter.IntegrationTests.Utils.Seeder.TestSeeder;

namespace VictoryCenter.IntegrationTests.Utils.Seeder;

internal static class IntegrationTestsDatabaseSeeder
{
    public static void SeedData(VictoryCenterDbContext dbContext)
    {
        TestDataSeeder.SeedData(dbContext);
        CategoriesDataSeeder.SeedData(dbContext);
        TeamMemberSeeder.SeedData(dbContext, dbContext.Categories.ToList());
    }

    public static void DeleteExistingData(VictoryCenterDbContext dbContext)
    {
        dbContext.TeamMembers.RemoveRange(dbContext.TeamMembers);
        dbContext.Categories.RemoveRange(dbContext.Categories);
        dbContext.TestEntities.RemoveRange(dbContext.TestEntities);
        dbContext.SaveChanges();
    }
}
