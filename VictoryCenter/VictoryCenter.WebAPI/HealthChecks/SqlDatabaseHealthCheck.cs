using Microsoft.Extensions.Diagnostics.HealthChecks;
using VictoryCenter.DAL.Data;

namespace VictoryCenter.WebAPI.HealthChecks;

public sealed class SqlDatabaseHealthCheck(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<SqlDatabaseHealthCheck> logger) : IHealthCheck
{
    private const string UnavailableDescription = "SQL database connection is unavailable";

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
            logger.LogWarning(exception, "SQL database readiness check failed");
            return HealthCheckResult.Unhealthy(UnavailableDescription);
        }
    }
}
