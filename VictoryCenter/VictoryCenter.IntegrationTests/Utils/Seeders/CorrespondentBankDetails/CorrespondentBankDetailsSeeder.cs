using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.CorrespondentBankDetails;

public class CorrespondentBankDetailsSeeder : BaseSeeder<Entities.CorrespondentBankDetails>
{
    public CorrespondentBankDetailsSeeder(
        VictoryCenterDbContext dbContext,
        ILogger<CorrespondentBankDetailsSeeder> logger)
        : base(dbContext, logger)
    {
    }

    public override string Name => "CorrespondentBankDetailsSeeder";

    public override int Order => (int)SeederExecutionOrder.CorrespondentBankDetails;

    protected override Task<List<Entities.CorrespondentBankDetails>> GenerateEntitiesAsync()
    {
        var list = new List<Entities.CorrespondentBankDetails>
        {
            new()
            {
                Id = 1,
                Name = "Correspondent Bank 1",
                Swift = "CORRSWIFT01",
                Account = "ACC1234567890",
                ForeignIban = "UA123456789012345678901234567",
                ForeignBankDetailsId = 1
            },
            new()
            {
                Id = 2,
                Name = "Correspondent Bank 2",
                Swift = "CORRSWIFT02",
                Account = "ACC9876543210",
                ForeignIban = "UA987654321098765432109876543",
                ForeignBankDetailsId = 1
            },
            new()
            {
                Id = 3,
                Name = "Correspondent Bank 3",
                Swift = "CORRSWIFT03",
                Account = "ACC1122334455",
                ForeignIban = null,
                ForeignBankDetailsId = 2
            }
        };

        return Task.FromResult(list);
    }
}
