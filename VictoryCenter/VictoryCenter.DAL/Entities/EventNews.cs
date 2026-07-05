using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class EventNews : BaseEntity, ITranslatedEntity<EventNewsLocalization>
{
    public string? Slug { get; set; }
    public string? Resource { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public Status Status { get; set; }
    public long? PreviewImageId { get; set; }
    public Image? PreviewImage { get; set; }
    public long? BackgroundImageId { get; set; }
    public Image? BackgroundImage { get; set; }
    public ICollection<EventNewsCategory> Categories { get; set; } = [];
    public ICollection<EventNewsLocalization> Localizations { get; set; } = [];
}
