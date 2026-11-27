using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ImpactStatistics.Metrics;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.Helpers;

public static class MetricValidationHelper
{
    public static bool HasRequiredKindComposition(IEnumerable<BaseMetricDto?> metrics)
    {
        var nonNullMetrics = metrics.OfType<BaseMetricDto>().ToList();

        return nonNullMetrics.Count(m => m.Kind == MetricKind.Standard) == MainPageConstants.ImpactStatistic.ExactStandardMetricCount
            && nonNullMetrics.Count(m => m.Kind == MetricKind.RaisedFunds) == MainPageConstants.ImpactStatistic.ExactRaisedFundsMetricCount;
    }
}
