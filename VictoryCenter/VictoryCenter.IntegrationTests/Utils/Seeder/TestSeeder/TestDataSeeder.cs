using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.Utils.Seeder.TestSeeder;

internal static class TestDataSeeder
{
    private static readonly List<TestEntity> _testEntities =
    [
        new () { TestName = "Test 1" },
        new () { TestName = "Test 2" },
        new () { TestName = "Test 3" },
    ];

    public static void SeedData(VictoryCenterDbContext dbContext)
        => dbContext.AddRange(_testEntities);
}
