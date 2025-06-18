using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.IntegrationTests.Utils.Seeder.TeamMembersSeeder;

internal static class TeamMemberSeeder
{
    private const int TeamMemberCount = 8;

    public static void SeedData(VictoryCenterDbContext dbContext, List<Category> categories)
    {
        for (var i = 0; i < TeamMemberCount; i++)
        {
            var teamMember = new TeamMember
            {
                FirstName = $"FirstName{i}",
                LastName = $"LastName{i}",
                MiddleName = $"MiddleName{i}",
                CategoryId = categories[i % (categories.Count - 2)].Id,
                Priority = i + 1,
                Status = (Status)(i % Enum.GetNames<Status>().Length),
                CreatedAt = new DateTime(2025, 2, 30 - i)
            };
            dbContext.TeamMembers.Add(teamMember);
        }

        dbContext.SaveChanges();
    }
}
