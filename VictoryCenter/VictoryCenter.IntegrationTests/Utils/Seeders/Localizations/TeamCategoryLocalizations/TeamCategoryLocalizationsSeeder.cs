using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.Localizations.TeamCategoryLocalizations;
public class TeamCategoryLocalizationsSeeder : BaseSeeder<DAL.Entities.Localization.TeamCategoryLocalization>
{
    public TeamCategoryLocalizationsSeeder(VictoryCenterDbContext dbContext, ILogger<TeamCategoryLocalizationsSeeder> logger)
        : base(dbContext, logger)
    {
    }

    public override int Order => (int)SeederExecutionOrder.TeamCategoryLocalizations;

    public override string Name => nameof(TeamCategoryLocalizationsSeeder);

    protected override Task<List<TeamCategoryLocalization>> GenerateEntitiesAsync()
    {
        var teamCategoryLocalizations = new List<TeamCategoryLocalization>
        {
            new()
            {
                EntityId = 1,
                LanguageId = 2,
                Name = "English Name",
                Description = "English Description"
            },
            new()
            {
                EntityId = 1,
                LanguageId = 3,
                Name = "English Name 4",
                Description = "English Description 4"
            },
            new()
            {
                EntityId = 2,
                LanguageId = 2,
                Name = "English Name 2",
                Description = "English Description 2"
            },
            new()
            {
                EntityId = 3,
                LanguageId = 2,
                Name = "English Name 3",
                Description = "English Description 3"
            }
        };
        return Task.FromResult(teamCategoryLocalizations);
    }
}
