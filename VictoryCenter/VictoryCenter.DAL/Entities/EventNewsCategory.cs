using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class EventNewsCategory : BaseEntity, ITranslatedEntity<EventNewsCategoryLocalization>
{
    [Audited]
    public string Name { get; set; } = null!;

    public ICollection<EventNews> EventsNews { get; set; } = [];

    public ICollection<EventNewsCategoryLocalization> Localizations { get; set; } = [];
}
