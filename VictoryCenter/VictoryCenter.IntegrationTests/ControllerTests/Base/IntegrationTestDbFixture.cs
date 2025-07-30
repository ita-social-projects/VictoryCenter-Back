using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Options;
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
    private readonly IServiceScope _scope;
    private readonly SeederManager _seederManager;

    public IntegrationTestDbFixture()
    {
        Factory = new VictoryCenterWebApplicationFactory<Program>();
        _scope = Factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        HttpClient = Factory.CreateClient();

        var loggerFactory = _scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        _seederManager = new SeederManager(DbContext, loggerFactory);

        DbContext.Database.EnsureCreated();
        _seederManager.SeedAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        EnsureTestAdminUser(_scope.ServiceProvider).ConfigureAwait(false).GetAwaiter().GetResult();

        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", GetAuthorizationToken(_scope.ServiceProvider));
    }

    public void Dispose()
    {
        _seederManager.DisposeAllAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        DbContext.Dispose();
        _scope.Dispose();
    }

    private string GetAuthorizationToken(IServiceProvider serviceProvider)
    {
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var logger = serviceProvider.GetRequiredService<ILogger<TokenService>>();
        var tokenService = new TokenService(jwtOptions, configuration, logger);

        return tokenService.CreateAccessToken(Array.Empty<Claim>());
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
                RefreshToken = "refresh_token",
                RefreshTokenValidTo = DateTime.MaxValue
            };
            var result = await userManager.CreateAsync(admin, TestPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create test admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}
