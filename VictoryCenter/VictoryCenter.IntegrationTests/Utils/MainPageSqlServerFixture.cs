using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using VictoryCenter.DAL.Data;
using VictoryCenter.IntegrationTests.Utils.DbFixture;

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
}

public sealed class SqlServerFactAttribute : FactAttribute
{
    public SqlServerFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(MainPageSqlServerFixture.ConnectionVariable)))
        {
            Skip = $"Set {MainPageSqlServerFixture.ConnectionVariable} to a disposable local SQL Server connection.";
        }
    }
}

public sealed class SqlServerTheoryAttribute : TheoryAttribute
{
    public SqlServerTheoryAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(MainPageSqlServerFixture.ConnectionVariable)))
        {
            Skip = $"Set {MainPageSqlServerFixture.ConnectionVariable} to a disposable local SQL Server connection.";
        }
    }
}
