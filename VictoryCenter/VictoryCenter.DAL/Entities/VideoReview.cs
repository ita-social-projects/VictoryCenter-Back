using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class VideoReview : BaseEntity, IOrderableEntity, ITranslatedEntity<VideoReviewLocalization>
{
    public string Title { get; set; } = null!;

    public string Link { get; set; } = null!;

    public long Priority { get; set; }

    public Status Status { get; set; }

    public ICollection<VideoReviewLocalization> Localizations { get; set; } = [];
}
