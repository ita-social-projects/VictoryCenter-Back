namespace VictoryCenter.BLL.Helpers;

public static class ValidationHelpers
{
    public static Func<long, bool> HaveMaximumDigits(int maxDigits)
    {
        return value => value.ToString().Length <= maxDigits;
    }

    public static Func<int, bool> HaveMaximumDigitsInt(int maxDigits)
    {
        return value => value.ToString().Length <= maxDigits;
    }
}
