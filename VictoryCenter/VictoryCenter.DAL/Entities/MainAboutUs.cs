using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class MainAboutUs : BaseEntity, ITranslatedEntity<MainAboutUsLocalization>
{
    [Audited]
    [RichText]
    public string Title { get; set; } = null!;

    [Audited]
    [RichText]
    public string Description { get; set; } = null!;

    [Audited]
    public long MainPageId { get; set; }

    public MainPage MainPage { get; set; } = null!;

    public ICollection<MainAboutUsLocalization> Localizations { get; set; } = [];
}
