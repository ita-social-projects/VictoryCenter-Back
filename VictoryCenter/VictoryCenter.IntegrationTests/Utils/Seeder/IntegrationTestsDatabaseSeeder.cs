using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.Utils.Seeder.CategoriesSeeder;
using VictoryCenter.IntegrationTests.Utils.Seeder.TeamMembersSeeder;

namespace VictoryCenter.IntegrationTests.Utils.Seeder;

internal static class IntegrationTestsDatabaseSeeder
{
    public static void SeedData(VictoryCenterDbContext dbContext)
    {
        CategoriesDataSeeder.SeedData(dbContext);
        TeamMemberSeeder.SeedData(dbContext, dbContext.Categories.ToList());
    }

    public static async Task DeleteExistingData(VictoryCenterDbContext dbContext)
    {
        dbContext.TeamMembers.RemoveRange(dbContext.TeamMembers);
        await dbContext.SaveChangesAsync();
        dbContext.Categories.RemoveRange(dbContext.Categories);
        dbContext.TestEntities.RemoveRange(dbContext.TestEntities);
        dbContext.SaveChanges();
    }
}
