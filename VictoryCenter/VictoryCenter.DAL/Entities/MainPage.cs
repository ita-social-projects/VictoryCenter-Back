using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

public class MainPage : BaseEntity, ITranslatedEntity<MainPageLocalization>
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;

    public long? ImageId { get; set; }
    public Image? Image { get; set; }

    public MainAboutUs? MainAboutUs { get; set; }
    public MainPartners? MainPartners { get; set; }
    public ImpactStatistics? ImpactStatistics { get; set; }

    public ICollection<MainPageLocalization> Localizations { get; set; } = [];
}
