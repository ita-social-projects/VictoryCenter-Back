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
        // Do not run recovery batches on the host's startup path.
        await Task.Yield();
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                await scope.ServiceProvider.GetRequiredService<StorageOperationProcessor>().ProcessBatchAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
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
                return;
            }
        }
    }
}
