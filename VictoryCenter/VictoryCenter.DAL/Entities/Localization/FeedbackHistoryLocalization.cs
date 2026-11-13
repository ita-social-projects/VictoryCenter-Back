using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class FeedbackHistoryLocalization : LocalizationBase<FeedbackHistory>
{
    [Audited]
    public string Title { get; set; } = null!;

    [Audited]
    public string Story { get; set; } = null!;
}
