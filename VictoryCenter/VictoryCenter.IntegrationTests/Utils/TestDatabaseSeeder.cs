// using VictoryCenter.DAL.Data;
// using VictoryCenter.DAL.Entities;
//
// namespace VictoryCenter.IntegrationTests.Utils;
//
// internal static class TestDatabaseSeeder
// {
//     private static readonly List<TestEntity> _testEntities = 
//     [
//         new () {TestName = "Test 1"},
//         new () {TestName = "Test 2"},
//         new () {TestName = "Test 3"},
//     ];
//
//     public static void SeedData(VictoryCenterDbContext dbContext)
//     {
//         dbContext.AddRange(_testEntities);
//         dbContext.SaveChanges();
//     }
//
//     public static void DeleteExistingData(VictoryCenterDbContext dbContext)
//     {
//         dbContext.TestEntities.RemoveRange(dbContext.TestEntities);
//         dbContext.SaveChanges();
//     }
// }