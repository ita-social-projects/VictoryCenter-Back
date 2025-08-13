using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Interfaces.BlobStorage;
using VictoryCenter.BLL.Services.BlobStorage;
using Microsoft.Extensions.Logging;
using VictoryCenter.BLL.Options;
using VictoryCenter.BLL.Services.TokenService;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils;
using VictoryCenter.IntegrationTests.Utils.Seeder;
using VictoryCenter.IntegrationTests.Utils.Seeders;

namespace VictoryCenter.IntegrationTests.ControllerTests.Base;

public class IntegrationTestDbFixture : IAsyncLifetime
{
    public VictoryCenterWebApplicationFactory<Program> _factory;
    public readonly IBlobService BlobService;
    public readonly BlobEnvironmentVariables BlobEnvironmentVariables;
    private IServiceScope _scope;
    private List<ISeeder> _seeders;
    private string _authToken;

    public IntegrationTestDbFixture()
    {
        _factory = new VictoryCenterWebApplicationFactory<Program>();
        _scope = _factory.Services.CreateScope();
        HttpClient = _factory.CreateClient();
        BlobService = _scope.ServiceProvider.GetRequiredService<IBlobService>();
        var options = _scope.ServiceProvider.GetRequiredService<IOptions<BlobEnvironmentVariables>>();
        BlobEnvironmentVariables = options.Value;

        EnsureTestAdminUser(_scope.ServiceProvider).ConfigureAwait(false).GetAwaiter().GetResult();

        _authToken = GetAuthorizationToken(_scope.ServiceProvider);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
    }

    public HttpClient HttpClient { get; private set; }
    public VictoryCenterDbContext DbContext { get; private set; }
    public SeederManager SeederManager { get; private set; }

    public async Task InitializeAsync()
    {
        await CreateFreshDatabase();
    }

    public async Task DisposeAsync()
    {
        if (SeederManager != null)
        {
            await SeederManager.DisposeAllAsync();
        }

        DbContext?.Dispose();

        if(_factory != null)
        {
            await _factory.DisposeAsync();
        }
    }

    public async Task CreateFreshDatabase()
    {
        if (_factory != null)
        {
            await _factory.DisposeAsync();
        }

        var databaseName = $"TestDb_{Guid.NewGuid()}_{DateTime.UtcNow.Ticks}";
        _factory = new VictoryCenterWebApplicationFactory<Program>(databaseName);

        _scope = _factory.Services.CreateScope();
        HttpClient = _factory.CreateClient();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);

        DbContext = _scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
        await DbContext.Database.EnsureCreatedAsync();

        await EnsureTestAdminUser(_scope.ServiceProvider);

        var loggerFactory = _scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var blobService = _scope.ServiceProvider.GetRequiredService<IBlobService>();
        SeederManager = new SeederManager(DbContext, loggerFactory, blobService);

        await SeederManager.SeedAllAsync();
        await DbContext.SaveChangesAsync();

        var teamMemberCount = await DbContext.TeamMembers.CountAsync();
        if (teamMemberCount == 0)
        {
            throw new InvalidOperationException("Database seeding failed - no TeamMembers were created");
        }
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
