using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class Metric : BaseEntity, ITranslatedEntity<MetricLocalization>, IOrderableEntity
{
    /// <summary>
    /// Name of the shadow property mapped to the legacy Type column (see <see cref="LegacyMetricType"/>).
    /// </summary>
    public const string LegacyTypeProperty = "Type";

    [Audited]
    public long StatisticId { get; set; }

    public ImpactStatistics Statistics { get; set; } = null!;

    [Audited]
    public int Value { get; set; }

    [Audited]
    public string Name { get; set; } = null!;

    // Computed by the database from the legacy Type column, so changes made in code are not saved.
    [Audited]
    public MetricKind Kind { get; set; }

    [Audited]
    public MetricPrefix? Prefix { get; set; }

    [Audited]
    public bool IsAutoSynced { get; set; }

    [Audited]
    public bool IsHidden { get; set; }

    [Audited]
    public long Priority { get; set; }

    public ICollection<MetricLocalization> Localizations { get; set; } = [];

    [System.ComponentModel.DataAnnotations.Timestamp]
    public byte[] RowVersion { get; set; } = [];
}
