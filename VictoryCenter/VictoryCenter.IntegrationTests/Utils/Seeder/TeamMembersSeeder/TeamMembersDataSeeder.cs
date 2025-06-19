using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.IntegrationTests.Utils.Seeder.TeamMembersSeeder;

internal static class TeamMembersDataSeeder
{
    private static List<TeamMember> _teamMembers = new()
    {
        new TeamMember
        {
            FirstName = "John",
            LastName = "Doe",
            MiddleName = "A.",
            CategoryId = 1,
            Priority = 1,
            Status = Status.Published,
            Description = "First test team member",
            CreatedAt = DateTime.Now,
            Email = "john.doe@example.com",
            Photo = null
        },
        new TeamMember
        {
            FirstName = "Jane",
            LastName = "Smith",
            MiddleName = "B.",
            CategoryId = 2,
            Priority = 2,
            Status = Status.Draft,
            Description = "Second test team member",
            CreatedAt = DateTime.Now,
            Email = "jane.smith@example.com",
            Photo = null
        },
        new TeamMember
        {
            FirstName = "Alex",
            LastName = "Johnson",
            MiddleName = "C.",
            CategoryId = 3,
            Priority = 3,
            Status = Status.Published,
            Description = "Third test team member",
            CreatedAt = DateTime.Now,
            Email = "alex.johnson@example.com",
            Photo = null
        }
    };

    public static void SeedData(VictoryCenterDbContext dbContext)
        => dbContext.AddRange(_teamMembers);
}
