using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class EventNews : BaseEntity, ITranslatedEntity<EventNewsLocalization>, IOrderableEntity
{
    public string? Slug { get; set; }
    public string? Resource { get; set; }
    public string? ResourceEn { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? AdditionalDescription { get; set; }
    public DateTimeOffset? PublishedAt { get; set; }
    public Status Status { get; set; }
    public long? PreviewImageId { get; set; }
    public Image? PreviewImage { get; set; }
    public long? BackgroundImageId { get; set; }
    public Image? BackgroundImage { get; set; }
    public long CategoryId { get; set; }
    public EventNewsCategory Category { get; set; } = null!;
    public long Priority { get; set; }
    public ICollection<EventNewsLocalization> Localizations { get; set; } = [];
}
