using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.ForeignBankDetails;
public class ForeignBankDetailsSeeder : BaseSeeder<Entities.ForeignBankDetails>
{
    public ForeignBankDetailsSeeder(VictoryCenterDbContext dbContext, ILogger<ForeignBankDetailsSeeder> logger)
        : base(dbContext, logger)
    {
    }

    public override string Name => "ForeignBankDetailsSeeder";
    public override int Order => (int)SeederExecutionOrder.ForeignBankDetails;

    protected override Task<List<Entities.ForeignBankDetails>> GenerateEntitiesAsync()
    {
        var list = new List<Entities.ForeignBankDetails>
        {
            new()
            {
                Id = 1,
                Name = "TestBank1",
                Receiver = "Charity Org UA",
                UkrainianIban = "UA123456789012345678901234567",
                Swift = "12345678901",
                Address = "Kyiv, Ukraine",
            },
            new()
            {
                Id = 2,
                Name = "TestBank2",
                Receiver = "Support Foundation",
                UkrainianIban = "UA123456789012345678901234567",
                Swift = "12345678901",
                Address = "Lviv, Ukraine",
            }
        };
        return Task.FromResult(list);
    }
}
