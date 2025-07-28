using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.Utils.Seeder.CategoriesSeeder;
using VictoryCenter.IntegrationTests.Utils.Seeder.TeamMembersSeeder;
using VictoryCenter.IntegrationTests.Utils.Seeder.ImageSeeder;

namespace VictoryCenter.IntegrationTests.Utils.Seeder;

internal static class IntegrationTestsDatabaseSeeder
{
    public static void SeedData(VictoryCenterDbContext dbContext, IBlobService blobService)
    {
        CategoriesDataSeeder.SeedData(dbContext);
        TeamMemberSeeder.SeedData(dbContext, dbContext.Categories.ToList());
        ImagesDataSeeder.SeedData(dbContext, blobService );
    }

    public static async Task DeleteExistingData(VictoryCenterDbContext dbContext, BlobEnvironmentVariables environment)
    {
        dbContext.Images.RemoveRange(dbContext.Images);
        await dbContext.SaveChangesAsync();
        Directory.Delete(environment.BlobStorePath, recursive: true);
        dbContext.TeamMembers.RemoveRange(dbContext.TeamMembers);
        await dbContext.SaveChangesAsync();
        dbContext.Categories.RemoveRange(dbContext.Categories);
        await dbContext.SaveChangesAsync();
    }
}
