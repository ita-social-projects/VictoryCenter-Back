using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;

namespace VictoryCenter.IntegrationTests.Utils.Seeders.Partners;

public class PartnersPageBannerSeeder : BaseSeeder<PartnersPageBanner>
{
    public PartnersPageBannerSeeder(VictoryCenterDbContext dbContext, ILogger<PartnersPageBannerSeeder> logger)
        : base(dbContext, logger)
    {
    }

    public override string Name => nameof(PartnersPageBannerSeeder);

    public override int Order => (int)SeederExecutionOrder.PartnersPageBanner;

    protected override async Task<List<PartnersPageBanner>> GenerateEntitiesAsync()
    {
        var image = await DbContext.Images.FirstOrDefaultAsync();
        if (image is null)
        {
            throw new InvalidOperationException("At least one image must be seeded before seeding the Partners Page Banner.");
        }

        var banner = new PartnersPageBanner
        {
            Title = "Our Trusted Partners",
            Description = "We are proud to collaborate with organizations that share our values and mission.",
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-30),
            ImageId = image.Id
        };

        return [banner];
    }
}
