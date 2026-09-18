using MediatR;

namespace VictoryCenter.BLL.Notifications.ReportFunds;

public class ReportFundsChangedNotification : INotification
{
    public bool SkipRaisedMetricSync { get; init; }
}
