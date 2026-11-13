using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class FeedbackHistory : BaseEntity, IOrderableEntity, ITranslatedEntity<FeedbackHistoryLocalization>
{
    [Audited]
    public string Title { get; set; } = null!;

    [Audited]
    public string Story { get; set; } = null!;

    [Audited]
    public long? ImageId { get; set; }

    public Image? Image { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    [Audited]
    public DateTimeOffset? PublishedAt { get; set; }

    [Audited]
    public long Priority { get; set; }

    [Audited]
    public Status Status { get; set; }

    public ICollection<FeedbackHistoryLocalization> Localizations { get; set; } = [];
}
