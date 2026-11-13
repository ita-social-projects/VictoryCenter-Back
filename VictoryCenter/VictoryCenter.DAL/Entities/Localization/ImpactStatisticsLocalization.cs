using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class ImpactStatisticsLocalization : LocalizationBase<ImpactStatistics>
{
    [Audited]
    public string? Title { get; set; }
}
