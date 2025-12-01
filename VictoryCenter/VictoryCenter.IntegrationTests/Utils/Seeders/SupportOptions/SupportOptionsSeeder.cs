using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.SupportOptions;
public class SupportOptionsSeeder : BaseSeeder<Entities.SupportOptions>
{
    public SupportOptionsSeeder(VictoryCenterDbContext dbContext, ILogger<SupportOptionsSeeder> logger)
        : base(dbContext, logger)
    {
    }

    public override string Name => "SupportOptionsSeeder";
    public override int Order => (int)SeederExecutionOrder.SupportOptions;

    protected override Task<List<Entities.SupportOptions>> GenerateEntitiesAsync()
    {
        var supportOptions = new List<Entities.SupportOptions>
        {
            new()
            {
                Id = 1,
                Name = "PhoneNumber",
                Value = "+380991112233",
            },
            new()
            {
                Id = 2,
                Name = "Email",
                Value = "support@victorycenter.com",
            }
        };
        return Task.FromResult(supportOptions);
    }
}
