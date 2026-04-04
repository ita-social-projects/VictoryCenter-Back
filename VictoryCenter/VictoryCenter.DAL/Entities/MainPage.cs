using VictoryCenter.DAL.Data.BaseEntity;
namespace VictoryCenter.DAL.Entities;

public class MainPage : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;

    public long? ImageId { get; set; }
    public Image? Image { get; set; }

    public MainAboutUs? MainAboutUs { get; set; }
    public MainPartners? MainPartners { get; set; }
    public ICollection<ImpactStatistics> ImpactStatistics { get; set; } = [];
}
