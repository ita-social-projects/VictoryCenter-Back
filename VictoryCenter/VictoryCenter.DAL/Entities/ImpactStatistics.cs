using VictoryCenter.DAL.Data.BaseEntity;
namespace VictoryCenter.DAL.Entities;

public class ImpactStatistics : BaseEntity
{
    public string Description { get; set; } = null!;
    public long? ImageId { get; set; }
    public Image? Image { get; set; }

    public long MainPageId { get; set; }
    public MainPage MainPage { get; set; } = null!;

    public ICollection<Metric> Metrics { get; set; } = [];
}
