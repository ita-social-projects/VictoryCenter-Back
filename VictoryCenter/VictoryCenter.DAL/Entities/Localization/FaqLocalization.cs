namespace VictoryCenter.DAL.Entities.Localization;

public class FaqLocalization : LocalizationBase<FaqQuestion>
{
    public string QuestionText { get; set; } = null!;

    public string AnswerText { get; set; } = null!;
}
