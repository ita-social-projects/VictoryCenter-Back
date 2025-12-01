using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.Partners;

public class PartnerSectionSeeder : BaseSeeder<PartnerSection>
{
    public PartnerSectionSeeder(VictoryCenterDbContext dbContext, ILogger<PartnerSectionSeeder> logger)
        : base(dbContext, logger)
    {
    }

    public override string Name => nameof(PartnerSectionSeeder);

    public override int Order => (int)SeederExecutionOrder.PartnersSections;

    protected override async Task<List<PartnerSection>> GenerateEntitiesAsync()
    {
        var sections = new List<PartnerSection>
        {
            new()
            {
                Title = "Strategic Partners",
                Description = "Our key partners who provide strategic support for our mission.",
                Priority = 1,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-20),
                Partners =
                [
                    new()
                    {
                        Description = "Global Support Foundation",
                        Priority = 1,
                        CreatedAt = DateTimeOffset.UtcNow.AddDays(-19)
                    },
                    new()
                    {
                        Description = "Innovate For Good Corp.",
                        Priority = 2,
                        CreatedAt = DateTimeOffset.UtcNow.AddDays(-18)
                    },
                    new()
                    {
                        Description = "Community Builders Alliance",
                        Priority = 3,
                        CreatedAt = DateTimeOffset.UtcNow.AddDays(-17),
                    }

                ]
            },
            new()
            {
                Title = "Media Supporters",
                Description = "Organizations that help us spread the word about our activities.",
                Priority = 2,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-15),
                Partners =
                [
                    new()
                    {
                        Description = "Truth News Network",
                        Priority = 1,
                        CreatedAt = DateTimeOffset.UtcNow.AddDays(-14)
                    },
                    new()
                    {
                        Description = "The Daily Chronicle",
                        Priority = 2,
                        CreatedAt = DateTimeOffset.UtcNow.AddDays(-13)
                    }

                ]
            },
            new()
            {
                Title = "Future Collaborators",
                Description = "We are always open to new partnerships.",
                Priority = 3,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-10),
                Partners = []
            }
        };

        return sections;
    }
}
