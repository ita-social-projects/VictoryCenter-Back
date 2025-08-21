using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
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
using VictoryCenter.IntegrationTests.Utils.Seeders;

namespace VictoryCenter.IntegrationTests.ControllerTests.DbFixture;

public class IntegrationTestDbFixture : IAsyncLifetime
{
    private IServiceScope _scope;

    public IntegrationTestDbFixture()
    {
        Factory = new VictoryCenterWebApplicationFactory<Program>();
        _scope = Factory.Services.CreateScope();
        HttpClient = Factory.CreateClient();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetAuthorizationToken(_scope.ServiceProvider));
    }

    public VictoryCenterWebApplicationFactory<Program> Factory { get; private set; }
    public BlobEnvironmentVariables BlobEnvironmentVariables { get; private set; } = null!;
    public HttpClient HttpClient { get; private set; }
    public VictoryCenterDbContext DbContext { get; private set; } = null!;
    public SeederManager SeederManager { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await CreateFreshWebApplication();
    }

    public async Task DisposeAsync()
    {
        if (SeederManager != null)
        {
            await SeederManager.DisposeAllAsync();
        }

        DbContext?.Dispose();

        if(Factory != null)
        {
            await Factory.DisposeAsync();
        }

        if (Directory.Exists(BlobEnvironmentVariables.FullPath))
        {
            Directory.Delete(BlobEnvironmentVariables.FullPath, recursive: true);
        }
    }

    public async Task CreateFreshWebApplication()
    {
        if (Factory != null)
        {
            await Factory.DisposeAsync();
        }

        InitializeServices();
        await InitializeDatabase();
        InitializeSeeders();
        await SeederManager.SeedAllAsync();
    }

    private void InitializeServices()
    {
        Factory = new VictoryCenterWebApplicationFactory<Program>();
        _scope = Factory.Services.CreateScope();
        HttpClient = Factory.CreateClient();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetAuthorizationToken(_scope.ServiceProvider));

        var options = _scope.ServiceProvider.GetRequiredService<IOptions<BlobEnvironmentVariables>>();
        BlobEnvironmentVariables = options.Value;

        DbContext = _scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
    }

    private async Task InitializeDatabase()
    {
        await DbContext.Database.EnsureCreatedAsync();
        await EnsureTestAdminUser(_scope.ServiceProvider);
    }

    private void InitializeSeeders()
    {
        var loggerFactory = _scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var blobService = _scope.ServiceProvider.GetRequiredService<IBlobService>();
        SeederManager = new SeederManager(DbContext, loggerFactory, blobService, _scope.ServiceProvider);
    }

    private static string GetAuthorizationToken(IServiceProvider serviceProvider)
    {
        var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var logger = serviceProvider.GetRequiredService<ILogger<TokenService>>();
        var tokenService = new TokenService(jwtOptions, configuration, logger);

        return tokenService.CreateAccessToken([]);
    }

    private static async Task EnsureTestAdminUser(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<Admin>>();
        const string testEmail = "testadmin@victorycenter.com";
        const string testPassword = "TestPassword123!";
        var existing = await userManager.FindByEmailAsync(testEmail);
        if (existing == null)
        {
            var admin = new Admin
            {
                UserName = testEmail,
                Email = testEmail,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                RefreshToken = "refresh_token",
                RefreshTokenValidTo = DateTime.MaxValue
            };
            var result = await userManager.CreateAsync(admin, testPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create test admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}
