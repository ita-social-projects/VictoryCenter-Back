namespace VictoryCenter.DAL.Entities.Localization;

public class MetricLocalization : LocalizationBase<Metric>
{
    public string? Value { get; set; }
    public string? Name { get; set; }
}
