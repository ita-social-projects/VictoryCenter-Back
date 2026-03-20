namespace VictoryCenter.BLL.Constants;

public static class ReportFundsExpendituresSettingsConstants
{
    public static readonly long SingletonSettingsId = 1;

    public static readonly int DisclaimerMinLength = 2;
    public static readonly int DisclaimerMaxLength = 1000;

    public static readonly decimal ExchangeRateMinValue = 0m;

    public static readonly int ExchangeRateDigitsBeforeDecimalPoint = 12;
    public static readonly int ExchangeRateDigitsAfterDecimalPoint = 6;

    public static readonly int ExchangeRatePrecision =
        ExchangeRateDigitsBeforeDecimalPoint + ExchangeRateDigitsAfterDecimalPoint;

    public static readonly int ExchangeRateScale = ExchangeRateDigitsAfterDecimalPoint;

    public static readonly string ExchangeRateFormat =
        $"a number with up to {ExchangeRateDigitsBeforeDecimalPoint} digits before the decimal separator and up to {ExchangeRateDigitsAfterDecimalPoint} after";
}
