using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VictoryCenter.BLL.Interfaces.Concurrency;
using VictoryCenter.BLL.Options;

namespace VictoryCenter.BLL.Services.Concurrency;

public sealed class ContentLockExpirationService : BackgroundService
{
    private readonly IContentLockManager _lockManager;
    private readonly IContentLockNotifier _notifier;
    private readonly ContentLockOptions _options;
    private readonly ILogger<ContentLockExpirationService> _logger;

    public ContentLockExpirationService(
        IContentLockManager lockManager,
        IContentLockNotifier notifier,
        IOptions<ContentLockOptions> options,
        ILogger<ContentLockExpirationService> logger)
    {
        _lockManager = lockManager;
        _notifier = notifier;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_options.ExpirationScanIntervalSeconds));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            foreach (var expired in _lockManager.ReleaseExpired())
            {
                try
                {
                    await _notifier.NotifyLockExpiredAsync(expired, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Unable to publish a content-lock expiration notification.");
                }
            }
        }
    }
}
