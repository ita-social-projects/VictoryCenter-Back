using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class ImpactStatistics : BaseEntity, ITranslatedEntity<ImpactStatisticsLocalization>
{
    [Audited]
    public string Title { get; set; } = null!;

    [Audited]
    public long? ImageId { get; set; }

    public Image? Image { get; set; }

    [Audited]
    public long MainPageId { get; set; }

    public MainPage MainPage { get; set; } = null!;

    public ICollection<Metric> Metrics { get; set; } = [];

    public ICollection<ImpactStatisticsLocalization> Localizations { get; set; } = [];
}
