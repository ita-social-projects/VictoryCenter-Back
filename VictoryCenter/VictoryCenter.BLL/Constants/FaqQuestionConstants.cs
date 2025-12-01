namespace VictoryCenter.BLL.Constants;

public static class FaqConstants
{
    public static readonly short QuestionTextMinLength = 10;
    public static readonly short QuestionTextMaxLength = 150;
    public static readonly short AnswerTextMinLength = 50;
    public static readonly short AnswerTextMaxLength = 1000;

    public static readonly string PageNotFoundOrContainsNoFaqQuestions = "VisitorPage not found or does not contain FaqQuestions";
    public static readonly string SomePagesNotFound = "Some VisitorPages were not found";
    public static readonly string IdsAreNonConsecutive = "OrderedIds are non-consecutive";
}
