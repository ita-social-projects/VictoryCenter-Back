using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.DAL.Data;
using VictoryCenter.WebAPI.Services;

namespace VictoryCenter.IntegrationTests.Utils;

public class VictoryCenterWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    private readonly string _databaseName;

    public VictoryCenterWebApplicationFactory(string? databaseName = null)
    {
        _databaseName = databaseName ?? $"TestDb_{Guid.NewGuid()}";
    }

    protected string TestBlobPath { get; } = Path.Combine(Path.GetTempPath(), "VictoryCenter_IntegrationTests_Blobs", Guid.NewGuid().ToString());

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        CleanupTestBlobDirectory();
        GC.SuppressFinalize(this);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Directory.CreateDirectory(TestBlobPath);
        builder.UseWebRoot(TestBlobPath);
        SetEnvironmentalVariables();

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.Sources.Clear();
            config.AddJsonFile("appsettings.IntegrationTests.json", optional: false);
            config.AddEnvironmentVariables();
            var dict = new Dictionary<string, string?>
            {
                ["JwtOptions:SecretKey"] = Environment.GetEnvironmentVariable("JWTOPTIONS_SECRETKEY")
                                           ?? throw new InvalidOperationException("JWTOPTIONS_SECRETKEY is not set in enviroment variables"),
                ["JwtOptions:RefreshTokenSecretKey"] = Environment.GetEnvironmentVariable("JWTOPTIONS_REFRESH_TOKEN_SECRETKEY")
                                                       ?? throw new InvalidOperationException("JWTOPTIONS_REFRESH_TOKEN_SECRETKEY is not set in configuration"),
                ["BlobEnvironmentVariables:ServiceType"] = "Local",
                ["BlobEnvironmentVariables:Local:BlobStoreKey"] = Environment.GetEnvironmentVariable("BLOB_LOCAL_STORE_KEY")
                    ?? throw new InvalidOperationException("BLOB_LOCAL_STORE_KEY is not set in environment variables"),
                ["BlobEnvironmentVariables:Local:BlobStorePath"] = TestBlobPath,
                ["CloudflareTurnstileCaptchaOptions:SecretKey"] = Environment.GetEnvironmentVariable("CLOUDFLARE_TURNSTILE_SECRET_KEY"),
                ["CloudflareTurnstileCaptchaOptions:SiteVerifyUrl"] = Environment.GetEnvironmentVariable("CLOUDFLARE_TURNSTILE_SITE_VERIFY_URL"),
                ["ContactUsEmailOptions:FromAddress"] = Environment.GetEnvironmentVariable("CONTACT_US_EMAIL_OPTIONS_FROM_ADDRESS")
            };

            config.AddInMemoryCollection(dict);
        });

        builder.ConfigureServices(services =>
        {
            var storageWorker = services.FirstOrDefault(descriptor => descriptor.ImplementationType == typeof(StorageOperationWorker));
            if (storageWorker != null)
            {
                services.Remove(storageWorker);
            }

            RemoveExistingContext(services);
            AddTestDbContext(services);

            var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            dbContext.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            CleanupTestBlobDirectory();
        }
    }

    private static void SetEnvironmentalVariables()
    {
        Environment.SetEnvironmentVariable("INITIAL_ADMIN_EMAIL", "secretadmintest@gmail.com");
        Environment.SetEnvironmentVariable("INITIAL_ADMIN_PASSWORD", "Pa$$w0rd!");
        Environment.SetEnvironmentVariable("JWTOPTIONS_SECRETKEY", "FFE4F83F-5D19-4FFD-831E-673B27DFB103");
        Environment.SetEnvironmentVariable("JWTOPTIONS_REFRESH_TOKEN_SECRETKEY", "1F9845EF-067D-4BEE-AD41-43EB8C089977");
        Environment.SetEnvironmentVariable(
            "DB_CONNECTION_STRING",
            "Server=localhost,1434;Database=VictoryCenter_IntegrationTests_Db;User Id=sa;Password=Your_password123;MultipleActiveResultSets=True;TrustServerCertificate=true");
        Environment.SetEnvironmentVariable("WAY4PAY_MERCHANT_LOGIN", "MOCK_VALUE");
        Environment.SetEnvironmentVariable("WAY4PAY_MERCHANT_SECRET_KEY", "MOCK_VALUE");
        Environment.SetEnvironmentVariable("WAY4PAY_MERCHANT_DOMAIN_NAME", "MOCK_VALUE");
        Environment.SetEnvironmentVariable("WAY4PAY_API_URL", "https://mock/pay");
        Environment.SetEnvironmentVariable("BLOB_LOCAL_STORE_KEY", "test-blob-key");
        Environment.SetEnvironmentVariable("CLOUDFLARE_TURNSTILE_SECRET_KEY", IntegrationTestConstants.CloudflareTurnstile.AlwaysPassesSecretKey);
        Environment.SetEnvironmentVariable("CLOUDFLARE_TURNSTILE_SITE_VERIFY_URL", "https://challenges.cloudflare.com/turnstile/v0/siteverify");
        Environment.SetEnvironmentVariable("CONTACT_US_EMAIL_OPTIONS_FROM_ADDRESS", "test@example.com");
    }

    private static void RemoveExistingContext(IServiceCollection services)
    {
        // EF Core registers provider configuration separately; remove it to avoid
        // mixing SQL Server and InMemory providers.
        var descriptors = services
            .Where(d =>
                d.ServiceType == typeof(DbContextOptions<VictoryCenterDbContext>) ||
                d.ServiceType == typeof(VictoryCenterDbContext) ||
                d.ServiceType == typeof(IDbContextOptionsConfiguration<VictoryCenterDbContext>))
            .ToList();

        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }
    }

    private void AddTestDbContext(IServiceCollection services)
    {
        var efServiceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        services.AddDbContext<VictoryCenterDbContext>(options =>
        {
            options
                .UseInMemoryDatabase(_databaseName)
                .UseInternalServiceProvider(efServiceProvider)
                .ConfigureWarnings(warnings =>
                    warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        });
    }

    private void CleanupTestBlobDirectory()
    {
        try
        {
            if (Directory.Exists(TestBlobPath))
            {
                var allowedRoot = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "VictoryCenter_IntegrationTests_Blobs")) + Path.DirectorySeparatorChar;
                if (!Path.GetFullPath(TestBlobPath).StartsWith(allowedRoot, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("The test storage path is outside the temporary test root.");
                }

                Directory.Delete(TestBlobPath, true);
            }
        }
        catch
        {
            // Ignore cleanup errors in tests
        }
    }
}
