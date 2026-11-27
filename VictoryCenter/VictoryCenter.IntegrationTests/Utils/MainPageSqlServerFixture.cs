using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.Utils.DbFixture;
using Xunit.Sdk;

namespace VictoryCenter.IntegrationTests.Utils;

public sealed class MainPageSqlServerFixture : IAsyncLifetime
{
    public const string ConnectionVariable = "VICTORYCENTER_TEST_SQLSERVER";

    private VictoryCenterWebApplicationFactory<Program>? _factory;

    public HttpClient Client { get; private set; } = null!;

    public IServiceProvider Services => _factory!.Services;

    public async Task InitializeAsync()
    {
        var connection = Environment.GetEnvironmentVariable(ConnectionVariable);
        if (string.IsNullOrWhiteSpace(connection))
        {
            if (IsRunningInCi())
            {
                throw new InvalidOperationException(
                    $"{ConnectionVariable} is not set. SQL Server tests are mandatory in CI.");
            }

            return;
        }

        var builder = new SqlConnectionStringBuilder(connection);
        if (!builder.DataSource.StartsWith("127.0.0.1,", StringComparison.OrdinalIgnoreCase) &&
            !builder.DataSource.StartsWith("localhost,", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("SQL integration tests require a disposable local SQL Server.");
        }

        builder.InitialCatalog = $"VictoryCenter_MetricsTests_{Guid.NewGuid():N}";
        _factory = new VictoryCenterWebApplicationFactory<Program>(sqlServerConnectionString: builder.ConnectionString);
        Client = _factory.CreateClient();
        Client.DefaultRequestHeaders.Authorization =
            await IntegrationTestDbFixture.CreateAdminAuthorizationHeaderAsync(Services);
    }

    public async Task DisposeAsync()
    {
        if (_factory is null)
        {
            return;
        }

        using (var scope = Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();
            await db.Database.EnsureDeletedAsync();
        }

        Client.Dispose();
        await _factory.DisposeAsync();
    }

    internal static bool IsRunningInCi() =>
        string.Equals(Environment.GetEnvironmentVariable("CI"), "true", StringComparison.OrdinalIgnoreCase);
}

[TraitDiscoverer("VictoryCenter.IntegrationTests.StorageLifecycle.SqlServerTraitDiscoverer", "VictoryCenter.IntegrationTests")]
public sealed class SqlServerFactAttribute : FactAttribute, ITraitAttribute
{
    public SqlServerFactAttribute()
    {
        if (!MainPageSqlServerFixture.IsRunningInCi() &&
            string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(MainPageSqlServerFixture.ConnectionVariable)))
        {
            Skip = $"Set {MainPageSqlServerFixture.ConnectionVariable} to a disposable local SQL Server connection.";
        }
    }
}

[TraitDiscoverer("VictoryCenter.IntegrationTests.StorageLifecycle.SqlServerTraitDiscoverer", "VictoryCenter.IntegrationTests")]
public sealed class SqlServerTheoryAttribute : TheoryAttribute, ITraitAttribute
{
    public SqlServerTheoryAttribute()
    {
        if (!MainPageSqlServerFixture.IsRunningInCi() &&
            string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(MainPageSqlServerFixture.ConnectionVariable)))
        {
            Skip = $"Set {MainPageSqlServerFixture.ConnectionVariable} to a disposable local SQL Server connection.";
        }
    }
}
