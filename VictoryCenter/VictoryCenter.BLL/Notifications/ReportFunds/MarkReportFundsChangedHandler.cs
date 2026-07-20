using MediatR;
using VictoryCenter.BLL.Helpers;
using VictoryCenter.DAL.Repositories.Interfaces.Base;

namespace VictoryCenter.BLL.Notifications.ReportFunds;

public class MarkReportFundsChangedHandler : INotificationHandler<ReportFundsChangedNotification>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly TimeProvider _timeProvider;

    public MarkReportFundsChangedHandler(IRepositoryWrapper repositoryWrapper, TimeProvider timeProvider)
    {
        _repositoryWrapper = repositoryWrapper;
        _timeProvider = timeProvider;
    }

    public async Task Handle(ReportFundsChangedNotification notification, CancellationToken cancellationToken)
    {
        var settingsResult = await ReportFundsExpendituresSettingsHelper
            .GetOrCreateSettingsAsync(_repositoryWrapper, _timeProvider);

        if (settingsResult.IsFailed)
        {
            return;
        }

        var settings = settingsResult.Value;

        if (!settings.HasUnpublishedChanges)
        {
            settings.HasUnpublishedChanges = true;
            _repositoryWrapper.ReportFundsExpendituresSettingsRepository.Update(settings);
            await _repositoryWrapper.SaveChangesAsync();
        }
    }
}
