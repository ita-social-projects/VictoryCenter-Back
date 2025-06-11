using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.IntegrationTests.Utils;

public static class VictoryCenterDatabaseSeeder
{
    public static async Task ClearDatabaseAsync(VictoryCenterDbContext dbContext)
    {
        dbContext.TeamMembers.RemoveRange(dbContext.TeamMembers);
        dbContext.Categories.RemoveRange(dbContext.Categories);
        await dbContext.SaveChangesAsync();
    }

    public static async Task SeedDataAsync(VictoryCenterDbContext dbContext)
    {
        await ClearDatabaseAsync(dbContext);

        var categories = new List<Category>
        {
            new() { Name = "Category 1", Description = "Description 1", CreatedAt = DateTime.UtcNow.AddDays(-10) },
            new() { Name = "Category 2", Description = "Description 2", CreatedAt = DateTime.UtcNow.AddDays(-20) },
            new() { Name = "Category 3", Description = "Description 3", CreatedAt = DateTime.UtcNow.AddDays(-30) },
        };

        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync();

        var teamMembers = new List<TeamMember>
        {
            new() { FirstName = "First", LastName = "Last", MiddleName = "Mid", CategoryId = categories[0].Id, Priority = 1, Status = Status.Draft },
            new() { FirstName = "Second", LastName = "Last", MiddleName = "Mid", CategoryId = categories[1].Id, Priority = 2, Status = Status.Draft },
        };

        await dbContext.TeamMembers.AddRangeAsync(teamMembers);
        await dbContext.SaveChangesAsync();
    }
}
