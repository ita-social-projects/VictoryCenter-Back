using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class FeedbackReview : BaseEntity, IOrderableEntity, ITranslatedEntity<FeedbackReviewLocalization>
{
    [Audited]
    public string AuthorName { get; set; } = null!;

    [Audited]
    public string Text { get; set; } = null!;

    [Audited]
    public Status Status { get; set; }

    [Audited]
    public long Priority { get; set; }

    public ICollection<FeedbackReviewLocalization> Localizations { get; set; } = [];
}
