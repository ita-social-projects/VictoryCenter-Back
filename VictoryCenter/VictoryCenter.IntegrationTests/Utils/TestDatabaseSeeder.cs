using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.Utils;

internal static class TestDatabaseSeeder
{
    public static void SeedData(VictoryCenterDbContext dbContext)
    {
        var testEntities = new List<TestEntity>
        {
            new() { TestName = "Test 1" },
            new() { TestName = "Test 2" },
            new() { TestName = "Test 3" },
        };

        dbContext.AddRange(testEntities);
        dbContext.SaveChanges();
    }

    public static void DeleteExistingData(VictoryCenterDbContext dbContext)
    {
        dbContext.TestEntities.RemoveRange(dbContext.TestEntities);
        dbContext.SaveChanges();
    }
}