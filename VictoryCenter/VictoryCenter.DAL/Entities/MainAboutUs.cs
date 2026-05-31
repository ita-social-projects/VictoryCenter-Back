using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

public class MainAboutUs : BaseEntity, ITranslatedEntity<MainAboutUsLocalization>
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;

    public long MainPageId { get; set; }
    public MainPage MainPage { get; set; } = null!;

    public ICollection<MainAboutUsLocalization> Localizations { get; set; } = [];
}
