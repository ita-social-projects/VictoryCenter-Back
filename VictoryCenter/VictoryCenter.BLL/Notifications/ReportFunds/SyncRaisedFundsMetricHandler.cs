using MediatR;
using Microsoft.EntityFrameworkCore;
using VictoryCenter.BLL.Services.FundsMetricSync;
using VictoryCenter.DAL.Entities;
using VictoryCenter.DAL.Enums;
using VictoryCenter.DAL.Repositories.Interfaces.Base;
using VictoryCenter.DAL.Repositories.Options;

namespace VictoryCenter.BLL.Notifications.ReportFunds;

public class SyncRaisedFundsMetricHandler : INotificationHandler<ReportFundsChangedNotification>
{
    private readonly IRepositoryWrapper _repositoryWrapper;

    private readonly IRaisedFundsMetricSyncService _syncService;

    public SyncRaisedFundsMetricHandler(IRepositoryWrapper repositoryWrapper, IRaisedFundsMetricSyncService syncService)
    {
        _repositoryWrapper = repositoryWrapper;
        _syncService = syncService;
    }

    public async Task Handle(ReportFundsChangedNotification notification, CancellationToken cancellationToken)
    {
        if (notification.SkipRaisedMetricSync)
        {
            return;
        }

        var raisedMetric = await _repositoryWrapper.MetricRepository.GetFirstOrDefaultAsync(
            new QueryOptions<Metric>
            {
                AsNoTracking = false,
                Filter = metric => metric.Type == MetricType.Raised,
                Include = query => query
                    .Include(metric => metric.Localizations)
                    .ThenInclude(localization => localization.Language)
            });

        if (raisedMetric is null)
        {
            return;
        }

        var changed = await _syncService.ApplySyncAsync(raisedMetric, cancellationToken);

        if (changed)
        {
            await _repositoryWrapper.SaveChangesAsync();
        }
    }
}
