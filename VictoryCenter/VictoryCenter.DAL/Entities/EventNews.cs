using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class EventNews : BaseEntity, ITranslatedEntity<EventNewsLocalization>, IOrderableEntity
{
    [Audited]
    public string? Slug { get; set; }

    [Audited]
    public string? Resource { get; set; }

    [Audited]
    public string? ResourceEn { get; set; }

    [Audited]
    public string? Title { get; set; }

    [Audited]
    public string? Description { get; set; }

    [Audited]
    public string? AdditionalDescription { get; set; }

    [Audited]
    public DateTimeOffset? PublishedAt { get; set; }

    [Audited]
    public Status Status { get; set; }

    [Audited]
    public long? PreviewImageId { get; set; }

    public Image? PreviewImage { get; set; }

    [Audited]
    public long? BackgroundImageId { get; set; }

    public Image? BackgroundImage { get; set; }

    [Audited]
    public long CategoryId { get; set; }

    public EventNewsCategory Category { get; set; } = null!;

    [Audited]
    public long Priority { get; set; }

    public ICollection<EventNewsLocalization> Localizations { get; set; } = [];
}
