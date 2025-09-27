using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using Entities = VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.UahBankDetails;
public class UahBankDetailsSeeder : BaseSeeder<Entities.UahBankDetails>
{
    public UahBankDetailsSeeder(VictoryCenterDbContext dbContext, ILogger<UahBankDetailsSeeder> logger)
        : base(dbContext, logger)
    {
    }

    public override string Name => "UahBankDetailsSeeder";
    public override int Order => (int)SeederExecutionOrder.UahBankDetails;

    protected override Task<List<Entities.UahBankDetails>> GenerateEntitiesAsync()
    {
        var list = new List<Entities.UahBankDetails>
        {
            new()
            {
                Id = 1,
                Name = "PrivatBank",
                Receiver = "PrivatBank",
                Edrpou = "11111111",
                Iban = "123456789012345678901234567",
                PaymentPurpose = "Donation"
            },
            new()
            {
                Id = 2,
                Name = "OschadBank",
                Receiver = "OschadBank",
                Edrpou = "11111111",
                Iban = "123456789012345678901234567",
                PaymentPurpose = "Donation"
            }
        };
        return Task.FromResult(list);
    }
}
