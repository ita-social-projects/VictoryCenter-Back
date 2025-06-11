using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.IntegrationTests.Utils;

internal static class VictoryCenterDatabaseSeeder
{
    public static void SeedData(VictoryCenterDbContext dbContext)
    {
        CategorySeeder.SeedCategories(dbContext);
        TeamMemberSeeder.SeedTeamMembers(dbContext, dbContext.Categories.ToList());
        dbContext.SaveChanges();
    }

    public static void DeleteExistingData(VictoryCenterDbContext dbContext)
    {
        TeamMemberSeeder.DeleteExistingTeamMembers(dbContext);
        CategorySeeder.DeleteExistingCategories(dbContext);
        dbContext.SaveChanges();
    }
}
