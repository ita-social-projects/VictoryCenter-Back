using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.Interfaces.TokenService;
using VictoryCenter.BLL.Services.BlobStorage;
using VictoryCenter.BLL.Services.PdfStorage;
using VictoryCenter.DAL.Data;
using VictoryCenter.DAL.Entities;
using VictoryCenter.IntegrationTests.Utils.Seeders;

namespace VictoryCenter.IntegrationTests.Utils.DbFixture;

public class IntegrationTestDbFixture : IAsyncLifetime
{
    private const string TestAdminEmail = "testadmin@victorycenter.com";
    private const string TestAdminPassword = "TestPassword123!";

    private IServiceScope _scope;

    public IntegrationTestDbFixture()
    {
        Factory = new VictoryCenterWebApplicationFactory<Program>();
        _scope = Factory.Services.CreateScope();
        HttpClient = Factory.CreateClient();
    }

    public VictoryCenterWebApplicationFactory<Program> Factory { get; private set; }
    public BlobEnvironmentVariables BlobEnvironmentVariables { get; private set; } = null!;
    public PdfEnvironmentVariables PdfEnvironmentVariables { get; private set; } = null!;
    public HttpClient HttpClient { get; private set; }
    public VictoryCenterDbContext DbContext { get; private set; } = null!;
    public SeederManager SeederManager { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await CreateFreshWebApplicationAsync();
    }

    public async Task DisposeAsync()
    {
        if (SeederManager != null)
        {
            await SeederManager.DisposeAllAsync();
        }

        DbContext?.Dispose();

        if (Factory != null)
        {
            await Factory.DisposeAsync();
        }

        if (Directory.Exists(BlobEnvironmentVariables.FullPath))
        {
            Directory.Delete(BlobEnvironmentVariables.FullPath, recursive: true);
        }

        if (Directory.Exists(PdfEnvironmentVariables.FullPath))
        {
            Directory.Delete(PdfEnvironmentVariables.FullPath, recursive: true);
        }
    }

    public static async Task<AuthenticationHeaderValue> CreateAdminAuthorizationHeaderAsync(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var admin = await EnsureTestAdminUserAsync(scope.ServiceProvider);
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        var accessToken = tokenService.CreateAccessToken([
            new Claim(ClaimTypes.Email, admin.Email!),
            new Claim(AuthConstants.SecurityStampHashClaimType, tokenService.HashSecurityStamp(admin.SecurityStamp!))
        ]);

        return new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public async Task CreateFreshWebApplicationAsync()
    {
        if (Factory != null)
        {
            await Factory.DisposeAsync();
        }

        InitializeServices();
        Directory.CreateDirectory(PdfEnvironmentVariables.FullPath);
        await InitializeDatabaseAsync();
        InitializeSeeders();
        await SeederManager.SeedAllAsync();
    }

    private void InitializeServices()
    {
        Factory = new VictoryCenterWebApplicationFactory<Program>();
        _scope = Factory.Services.CreateScope();
        HttpClient = Factory.CreateClient();

        var options = _scope.ServiceProvider.GetRequiredService<IOptions<BlobEnvironmentVariables>>();
        var pdfOptions = _scope.ServiceProvider.GetRequiredService<IOptions<PdfEnvironmentVariables>>();
        PdfEnvironmentVariables = pdfOptions.Value;
        BlobEnvironmentVariables = options.Value;

        DbContext = _scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
    }

    private async Task InitializeDatabaseAsync()
    {
        await DbContext.Database.EnsureCreatedAsync();
        HttpClient.DefaultRequestHeaders.Authorization = await CreateAdminAuthorizationHeaderAsync(_scope.ServiceProvider);
    }

    private void InitializeSeeders()
    {
        SeederManager = new SeederManager(_scope.ServiceProvider);
    }

    private static async Task<AdminUser> EnsureTestAdminUserAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<AdminUser>>();
        var existing = await userManager.FindByEmailAsync(TestAdminEmail);
        if (existing != null)
        {
            return existing;
        }

        var admin = new AdminUser
        {
            UserName = TestAdminEmail,
            Email = TestAdminEmail,
            EmailConfirmed = true,
            CreatedAt = DateTimeOffset.UtcNow,
            RefreshToken = "refresh_token",
            RefreshTokenValidTo = DateTimeOffset.MaxValue
        };
        var result = await userManager.CreateAsync(admin, TestAdminPassword);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to create test admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        return admin;
    }
}
