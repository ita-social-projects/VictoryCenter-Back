using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class FeedbackReview : BaseEntity, IOrderableEntity, ITranslatedEntity<FeedbackReviewLocalization>
{
    public string AuthorName { get; set; } = null!;

    public string Text { get; set; } = null!;

    public Status Status { get; set; }

    public long Priority { get; set; }

    public ICollection<FeedbackReviewLocalization> Localizations { get; set; } = [];
}
