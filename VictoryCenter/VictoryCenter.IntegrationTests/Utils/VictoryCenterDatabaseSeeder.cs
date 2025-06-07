using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.IntegrationTests.Utils;

internal static class VictoryCenterDatabaseSeeder
{
    private static readonly List<Category> _testCategories =
    [
        new () {Id = 1, Name = "Category 1", Description = "Category 1 Decription", CreatedAt = DateTime.Now.AddDays(-10)},
        new () {Id = 2, Name = "Category 2", Description = "Category 2 Decription", CreatedAt = DateTime.Now.AddDays(-20)},
        new () {Id = 3, Name = "Category 3", Description = "Category 3 Decription", CreatedAt = DateTime.Now.AddDays(-30)},
    ];

    private static readonly List<TeamMember> _testTeamMembers =
    [
        new () {
            Id = 1, FirstName = "FirstName1", LastName="LastName1", MiddleName = "MiddleName1",
            CategoryId = 1, Priority = 1, Status = Status.Draft, CreatedAt = DateTime.Now.AddDays(-1) },
        new () {
            Id = 2, FirstName = "FirstName2", LastName="LastName2", MiddleName = "MiddleName2",
            CategoryId = 2, Priority = 2, Status = Status.Draft, CreatedAt = DateTime.Now.AddDays(-2) },
        new () {
            Id = 3, FirstName = "FirstName3", LastName="LastName3", MiddleName = "MiddleName3",
            CategoryId = 3, Priority = 3, Status = Status.Draft, CreatedAt = DateTime.Now.AddDays(-3) }
    ];

    public static void SeedData(VictoryCenterDbContext dbContext)
    {
        dbContext.AddRange(_testCategories);
        dbContext.AddRange(_testTeamMembers);
        dbContext.SaveChanges();
    }

    public static void DeleteExistingData(VictoryCenterDbContext dbContext)
    {
        dbContext.Categories.RemoveRange(dbContext.Categories);
        dbContext.TeamMembers.RemoveRange(dbContext.TeamMembers);
        dbContext.SaveChanges();
    }
}
