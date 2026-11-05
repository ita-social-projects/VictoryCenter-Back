using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Services.StorageLifecycle;

namespace VictoryCenter.WebAPI.Services;

public class StorageReconciliationWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<StorageLifecycleOptions> options,
    TimeProvider clock,
    ILogger<StorageReconciliationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // A full inventory scan must not block startup or the cleanup worker.
        await Task.Yield();
        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = TimeSpan.FromSeconds(options.Value.PollIntervalSeconds);
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var findings = await scope.ServiceProvider.GetRequiredService<StorageReconciliation>().ScanAsync(stoppingToken);
                if (findings > 0)
                {
                    logger.LogWarning("Storage reconciliation observed {FindingCount} missing or unreferenced files", findings);
                }

                delay = TimeSpan.FromMinutes(options.Value.ReconciliationIntervalMinutes);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception exception)
            {
                logger.LogError("Storage reconciliation failed with {ErrorCode}; the scan will be retried", exception.GetType().Name);
            }

            try
            {
                await Task.Delay(delay, clock, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
        }
    }
}
