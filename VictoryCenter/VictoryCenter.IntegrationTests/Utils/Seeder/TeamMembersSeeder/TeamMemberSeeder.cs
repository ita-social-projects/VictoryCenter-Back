using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.IntegrationTests.Utils.Seeder.TeamMembersSeeder;

public class TeamMembersSeeder : BaseSeeder<TeamMember>
{
    private const int TeamMemberCount = 8;

    public TeamMembersSeeder(VictoryCenterDbContext dbContext, ILogger<TeamMembersSeeder> logger, IBlobService blobService)
        : base(dbContext, logger, blobService)
    {
    }

    public override string Name => "TeamMembersSeeder";

    public override int Order => 2;

    protected override async Task<bool> ShouldSkipAsync()
    {
        return await _dbContext.TeamMembers.AnyAsync();
    }

    protected override async Task<List<TeamMember>> GenerateEntitiesAsync()
    {
        var categories = await _dbContext.Categories.ToListAsync();
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
