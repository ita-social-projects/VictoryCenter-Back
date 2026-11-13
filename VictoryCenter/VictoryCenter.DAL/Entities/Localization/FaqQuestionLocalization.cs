using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class FaqQuestionLocalization : LocalizationBase<FaqQuestion>
{
    [Audited]
    public string QuestionText { get; set; } = null!;

    [Audited]
    public string AnswerText { get; set; } = null!;
}
