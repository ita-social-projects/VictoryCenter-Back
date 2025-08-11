using dotenv.net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.DAL.Data;

namespace VictoryCenter.IntegrationTests.Utils;

public class VictoryCenterWebApplicationFactory<TSartup> : WebApplicationFactory<TSartup>
    where TSartup : class
{
    private static readonly string TestBlobPath = Path.Combine(Path.GetTempPath(), "VictoryCenter_IntegrationTests_Blobs", Guid.NewGuid().ToString());
    private readonly string _databaseName;

    public VictoryCenterWebApplicationFactory(string databaseName = null)
    {
        _databaseName = databaseName ?? $"TestDb_{Guid.NewGuid()}";
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var envPath = Path.GetFullPath("../../../../VictoryCenter.WebApi/.env");

        DotEnv.Load(new DotEnvOptions(envFilePaths: new[] { envPath }));

        SetEnvironmentalVariables();

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();
            config.AddJsonFile("appsettings.IntegrationTests.json", optional: false);
            config.AddEnvironmentVariables();
            var dict = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("INTEGRATION_TESTS_DB_CONNECTION_STRING")
                                                          ?? throw new InvalidOperationException("INTEGRATION_TESTS_DB_CONNECTION_STRING is not set in enviroment variables"),
                ["JwtOptions:SecretKey"] = Environment.GetEnvironmentVariable("JWTOPTIONS_SECRETKEY")
                                           ?? throw new InvalidOperationException("JWTOPTIONS_SECRETKEY is not set in enviroment variables"),
                ["JwtOptions:RefreshTokenSecretKey"] = Environment.GetEnvironmentVariable("JWTOPTIONS_REFRESH_TOKEN_SECRETKEY")
                                                       ?? throw new InvalidOperationException("JWTOPTIONS_REFRESH_TOKEN_SECRETKEY is not set in configuration"),
                ["BlobEnvironmentVariables:ServiceType"] = "Local",
                ["BlobEnvironmentVariables:Local:BlobStoreKey"] = Environment.GetEnvironmentVariable("BLOB_LOCAL_STORE_KEY")
                    ?? throw new InvalidOperationException("BLOB_LOCAL_STORE_KEY is not set in environment variables"),
                ["BlobEnvironmentVariables:Local:BlobStorePath"] = TestBlobPath
            };

            config.AddInMemoryCollection(dict);
        });

        builder.ConfigureServices(services =>
        {
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
        if (disposing)
        {
            CleanupTestBlobDirectory();
        }

        base.Dispose(disposing);
    }

    private void SetEnvironmentalVariables()
    {
        Environment.SetEnvironmentVariable("INITIAL_ADMIN_EMAIL", "secretadmintest@gmail.com");
        Environment.SetEnvironmentVariable("INITIAL_ADMIN_PASSWORD", "Pa$$w0rd!");
        Environment.SetEnvironmentVariable("JWTOPTIONS_SECRETKEY", "FFE4F83F-5D19-4FFD-831E-673B27DFB103");
        Environment.SetEnvironmentVariable("JWTOPTIONS_REFRESH_TOKEN_SECRETKEY", "1F9845EF-067D-4BEE-AD41-43EB8C089977");
        Environment.SetEnvironmentVariable("DB_CONNECTION_STRING", "Server=localhost,1434;Database=VictoryCenter_IntegrationTests_Db;User Id=sa;Password=Admin@1234;MultipleActiveResultSets=True;TrustServerCertificate=true");
        Environment.SetEnvironmentVariable("WAY4PAY_MERCHANT_LOGIN", "MOCK_VALUE");
        Environment.SetEnvironmentVariable("WAY4PAY_MERCHANT_SECRET_KEY", "MOCK_VALUE");
        Environment.SetEnvironmentVariable("WAY4PAY_MERCHANT_DOMAIN_NAME", "MOCK_VALUE");
        Environment.SetEnvironmentVariable("WAY4PAY_API_URL", "https://mock/pay");
        Environment.SetEnvironmentVariable("BLOB_LOCAL_STORE_KEY", "test-blob-key");
    }

    private static void RemoveExistingContext(IServiceCollection services)
    {
        var descriptors = services
            .Where(d =>
                d.ServiceType == typeof(DbContextOptions<VictoryCenterDbContext>) ||
                d.ServiceType == typeof(VictoryCenterDbContext))
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
            options.UseInMemoryDatabase(_databaseName)
                .UseInternalServiceProvider(efServiceProvider);
        });
    }

    private static void CleanupTestBlobDirectory()
    {
        try
        {
            if (Directory.Exists(TestBlobPath))
            {
                Directory.Delete(TestBlobPath, true);
            }
        }
        catch
        {
            // Ignore cleanup errors in tests
        }
    }
}
