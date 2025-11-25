namespace VictoryCenter.DAL.Entities.Localization;

public class FaqQuestionLocalization : LocalizationBase<FaqQuestion>
{
    public string QuestionText { get; set; } = null!;

    public string AnswerText { get; set; } = null!;
}
