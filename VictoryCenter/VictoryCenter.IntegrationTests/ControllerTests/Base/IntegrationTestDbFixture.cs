using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.Seeder;

namespace VictoryCenter.IntegrationTests.ControllerTests.Base;

public class IntegrationTestDbFixture : IDisposable
{
    public readonly HttpClient HttpClient;
    public readonly VictoryCenterDbContext DbContext;
    public readonly VictoryCenterWebApplicationFactory<Program> Factory;

    public IntegrationTestDbFixture()
    {
        Factory = new VictoryCenterWebApplicationFactory<Program>();
        var scope = Factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        HttpClient = Factory.CreateClient();

        DbContext.Database.EnsureDeleted();
        DbContext.Database.Migrate();
        IntegrationTestsDatabaseSeeder.SeedData(DbContext);
    }

    public void Dispose()
    {
        IntegrationTestsDatabaseSeeder.DeleteExistingData(DbContext);
        DbContext.Dispose();
    }
}
