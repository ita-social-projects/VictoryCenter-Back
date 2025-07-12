using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.Seeder;

namespace VictoryCenter.IntegrationTests.ControllerTests.Base;

public class IntegrationTestDbFixture : IDisposable
{
    public readonly HttpClient HttpClient;
    public readonly VictoryCenterDbContext DbContext;
    public readonly VictoryCenterWebApplicationFactory<Program> Factory;
    public readonly IBlobService BlobService;
    public readonly BlobEnvironmentVariables BlobVariables;

    public IntegrationTestDbFixture()
    {
        Factory = new VictoryCenterWebApplicationFactory<Program>();
        var scope = Factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        HttpClient = Factory.CreateClient();
        BlobService = scope.ServiceProvider.GetRequiredService<IBlobService>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<BlobEnvironmentVariables>>();
        BlobVariables = options.Value;

        DbContext.Database.EnsureDeleted();
        DbContext.Database.Migrate();
        IntegrationTestsDatabaseSeeder.SeedData(DbContext, BlobService);
    }

    public void Dispose()
    {
        IntegrationTestsDatabaseSeeder.DeleteExistingData(DbContext);
        DbContext.Dispose();
    }
}
