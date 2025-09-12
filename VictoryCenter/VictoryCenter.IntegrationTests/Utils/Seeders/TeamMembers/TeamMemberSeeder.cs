using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.TeamMembers;

public class TeamMembersSeeder : BaseSeeder<TeamMember>
{
    private const int TeamMemberCount = 8;

    public TeamMembersSeeder(VictoryCenterDbContext dbContext, ILogger<TeamMembersSeeder> logger)
        : base(dbContext, logger)
    {
    }

    public override string Name => nameof(TeamMembersSeeder);

    public override int Order => (int)SeederExecutionOrder.TeamMembers;

    protected override async Task<List<TeamMember>> GenerateEntitiesAsync()
    {
        var categories = await DbContext.Categories.ToListAsync();
        if (categories.Count < 2)
        {
            throw new InvalidOperationException("At least 2 categories required to seed team members.");
        }

        var selectedCategories = categories.Take(2).ToList();

        var teamMembers = new List<TeamMember>();

        for (int i = 0; i < TeamMemberCount; i++)
        {
            var category = selectedCategories[i % selectedCategories.Count];
            teamMembers.Add(new TeamMember
            {
                FullName = $"FirstName{i} LastName{i}",
                CategoryId = category.Id,
                Priority = i + 1,
                Status = (Status)(i % Enum.GetNames<Status>().Length),
                CreatedAt = DateTime.UtcNow.AddMinutes(-10 * i)
            });
        }

        return teamMembers;
    }
}
