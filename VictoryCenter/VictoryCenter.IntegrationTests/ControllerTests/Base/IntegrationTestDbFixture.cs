using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Options;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.BLL.Services.TokenService;
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
        EnsureTestAdminUser(scope.ServiceProvider).ConfigureAwait(false).GetAwaiter().GetResult();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetAuthorizationToken(scope.ServiceProvider));
        IntegrationTestsDatabaseSeeder.SeedData(DbContext, BlobService);
    }

    public void Dispose()
    {
        IntegrationTestsDatabaseSeeder.DeleteExistingData(DbContext, BlobVariables);
        DbContext.Dispose();
    }

    private string GetAuthorizationToken(IServiceProvider serviceProvider)
    {
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var logger = serviceProvider.GetRequiredService<ILogger<TokenService>>();
        var tokenService = new TokenService(jwtOptions, configuration, logger);

        return tokenService.CreateAccessToken([]);
    }

    private async Task EnsureTestAdminUser(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<AdminUser>>();
        const string TestEmail = "testadmin@victorycenter.com";
        const string TestPassword = "TestPassword123!";
        var existing = await userManager.FindByEmailAsync(TestEmail);
        if (existing == null)
        {
            var admin = new AdminUser
            {
                UserName = TestEmail,
                Email = TestEmail,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                RefreshToken = "refresh_token",
                RefreshTokenValidTo = DateTime.MaxValue
            };
            var result = await userManager.CreateAsync(admin, TestPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create test admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}
