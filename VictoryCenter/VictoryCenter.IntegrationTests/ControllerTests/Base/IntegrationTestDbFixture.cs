using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
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
        EnsureTestAdminUser(scope.ServiceProvider).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        IntegrationTestsDatabaseSeeder.DeleteExistingData(DbContext);
        DbContext.Dispose();
    }

    private async Task EnsureTestAdminUser(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<Admin>>();
        const string TestEmail = "testadmin@victorycenter.com";
        const string TestPassword = "TestPassword123!";
        var existing = await userManager.FindByEmailAsync(TestEmail);
        if (existing == null)
        {
            var admin = new Admin
            {
                UserName = TestEmail,
                Email = TestEmail,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                RefreshToken = "refresh_token"
            };
            var result = await userManager.CreateAsync(admin, TestPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create test admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}
