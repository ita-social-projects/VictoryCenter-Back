namespace VictoryCenter.BLL.Constants;

public static class ReportFundsExpendituresRecordConstants
{
    public static readonly decimal AmountMinValue = 0m;

    public static readonly int AmountDigitsBeforeDecimalPoint = 11;
    public static readonly int AmountDigitsAfterDecimalPoint = 2;

    public static readonly int AmountPrecision =
        AmountDigitsBeforeDecimalPoint + AmountDigitsAfterDecimalPoint;

    public static readonly int AmountScale = AmountDigitsAfterDecimalPoint;

    public static readonly string AmountFormat =
        $"a number with up to {AmountDigitsBeforeDecimalPoint} digits before the decimal separator and up to {AmountDigitsAfterDecimalPoint} after";

    public static readonly string CategoryTypeMustMatchRecordType =
        "Category type must match record type";

    public static readonly string CategoryAlreadyHasRecord =
        "Record for this category already exists";
    public static int ReportingYearMinValue => System.DateTime.UtcNow.Year - 1;
    public static int ReportingYearMaxValue => System.DateTime.UtcNow.Year + 1;
}
