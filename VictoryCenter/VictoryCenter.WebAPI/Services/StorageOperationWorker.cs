using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Services.StorageLifecycle;

namespace VictoryCenter.WebAPI.Services;

public class StorageOperationWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<StorageLifecycleOptions> options,
    TimeProvider clock,
    ILogger<StorageOperationWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Do not perform filesystem work on the host's startup path.
        await Task.Yield();
        var nextReconciliation = clock.GetUtcNow();
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                await scope.ServiceProvider.GetRequiredService<StorageOperationProcessor>().ProcessBatchAsync(stoppingToken);
                if (clock.GetUtcNow() >= nextReconciliation)
                {
                    var findings = await scope.ServiceProvider.GetRequiredService<StorageReconciliation>().ScanAsync(stoppingToken);
                    if (findings > 0)
                    {
                        logger.LogWarning("Storage reconciliation observed {FindingCount} missing or unreferenced files", findings);
                    }

                    nextReconciliation = clock.GetUtcNow().AddMinutes(options.Value.ReconciliationIntervalMinutes);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError("Storage worker batch failed with {ErrorCode}; pending operations remain durable", exception.GetType().Name);
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(options.Value.PollIntervalSeconds), clock, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }
}
