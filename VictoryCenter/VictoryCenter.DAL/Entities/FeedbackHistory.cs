using VictoryCenter.DAL.Data.BaseEntity;
using VictoryCenter.DAL.Entities.Interfaces;
using VictoryCenter.DAL.Entities.Localization;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class FeedbackHistory : BaseEntity, IOrderableEntity, ITranslatedEntity<FeedbackHistoryLocalization>
{
    public string Title { get; set; } = null!;
    public string Story { get; set; } = null!;
    public long? ImageId { get; set; }
    public Image? Image { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public long Priority { get; set; }
    public Status Status { get; set; }
    public ICollection<FeedbackHistoryLocalization> Localizations { get; set; } = [];
}
