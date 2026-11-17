using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class VideoReview : BaseEntity, IOrderableEntity, ITranslatedEntity<VideoReviewLocalization>
{
    [Audited]
    public string Title { get; set; } = null!;

    [Audited]
    public string Link { get; set; } = null!;

    [Audited]
    public bool IsArchived { get; set; }

    [Audited]
    public DateTimeOffset? ArchivedAt { get; set; }

    [Audited]
    public long Priority { get; set; }

    [Audited]
    public Status Status { get; set; }

    public ICollection<VideoReviewLocalization> Localizations { get; set; } = [];
}
