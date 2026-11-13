using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class MetricLocalization : LocalizationBase<Metric>
{
    [Audited]
    public string? Value { get; set; }

    [Audited]
    public string? Name { get; set; }
}
