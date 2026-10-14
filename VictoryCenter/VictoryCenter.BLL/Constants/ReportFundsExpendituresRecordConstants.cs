namespace VictoryCenter.BLL.Constants;

public static class ReportFundsExpendituresRecordConstants
{
    public static readonly decimal ZeroAmount = 0m;
    public static readonly int AmountDigitsBeforeDecimalPoint = 9;
    public static readonly int AmountDigitsAfterDecimalPoint = 2;

    public static readonly int AmountPrecision =
        AmountDigitsBeforeDecimalPoint + AmountDigitsAfterDecimalPoint;

    public static readonly int AmountScale = AmountDigitsAfterDecimalPoint;

    public static readonly string AmountFormat =
        $"a number with up to {AmountDigitsBeforeDecimalPoint} digits before the decimal separator and up to {AmountDigitsAfterDecimalPoint} after";

    public static readonly string CategoryTypeMustMatchRecordType =
        "Category type must match record type";

    public static readonly int MaxNumberOfRecordsPerBulkDelete = 100;

    public static readonly int MaxNumberOfRecordsPerBatchOperation = 100;

    public static string CategoryAlreadyHasRecord()
    {
        return "Record for this category already exists";
    }

    public static string CategoryAlreadyHasRecord(long categoryId)
    {
        return $"Record for the {categoryId} category already exists";
    }
}
