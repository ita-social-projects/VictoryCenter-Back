using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class Metric : BaseEntity, ITranslatedEntity<MetricLocalization>
{
    public long StatisticId { get; set; }
    public ImpactStatistics Statistics { get; set; } = null!;
    public int Value { get; set; }
    public string Name { get; set; } = null!;
    public MetricType Type { get; set; }
    public MetricPrefix? Prefix { get; set; }
    public ICollection<MetricLocalization> Localizations { get; set; } = [];
}
