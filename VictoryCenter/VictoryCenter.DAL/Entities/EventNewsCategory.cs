using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

public class EventNewsCategory : BaseEntity, ITranslatedEntity<EventNewsCategoryLocalization>
{
    public string Name { get; set; } = null!;
    public ICollection<EventNews> EventsNews { get; set; } = [];
    public ICollection<EventNewsCategoryLocalization> Localizations { get; set; } = [];
}
