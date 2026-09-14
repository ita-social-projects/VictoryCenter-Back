namespace VictoryCenter.DAL.Entities.Localization;

public class FeedbackHistoryLocalization : LocalizationBase<FeedbackHistory>
{
    public string Title { get; set; } = null!;

    public string Story { get; set; } = null!;
}
