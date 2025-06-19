using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.IntegrationTests.Utils.Seeder.TeamMembersSeeder;

internal static class TeamMemberSeeder
{
    private const int TeamMemberCount = 8;

    public static void SeedData(VictoryCenterDbContext dbContext, List<Category> categories)
    {
        for (int i = 0; i < TeamMemberCount; i++)
        {
            var teamMember = new TeamMember
            {
                FirstName = $"FirstName{i}",
                LastName = $"LastName{i}",
                MiddleName = $"MiddleName{i}",
                CategoryId = categories[i % (categories.Count - 1)].Id,
                Priority = i + 1,
                Status = (Status)(i % Enum.GetNames<Status>().Length),
                CreatedAt = DateTime.UtcNow.AddMinutes(-10 * i)
            };
            dbContext.TeamMembers.Add(teamMember);
        }

        dbContext.SaveChanges();
    }
}
