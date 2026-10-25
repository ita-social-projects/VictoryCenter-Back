using System.Globalization;
using System.Resources;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using VictoryCenter.DAL.Data;

namespace VictoryCenter.WebAPI.HealthChecks;

public sealed class SqlDatabaseHealthCheck(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<SqlDatabaseHealthCheck> logger) : IHealthCheck
{
    private const string UnavailableDescription = "SQL database connection is unavailable";

    private static readonly ResourceManager LogMessages = new(
        "VictoryCenter.WebAPI.HealthChecks.HealthCheckMessages",
        typeof(SqlDatabaseHealthCheck).Assembly);

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<VictoryCenterDbContext>();

            return await dbContext.Database.CanConnectAsync(cancellationToken)
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy(UnavailableDescription);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, LogMessages.GetString("SqlReadinessCheckFailed", CultureInfo.InvariantCulture));
            return HealthCheckResult.Unhealthy(UnavailableDescription);
        }
    }
}
