namespace VictoryCenter.BLL.Constants;
public static class ForeignBankDetailsConstants
{
    public static readonly string OnlyDigitsMessage = "only digits allowed";
    public static readonly string OnlyDigits = "^[0-9]+$";
    public static class Swift
    {
        public static readonly int MaxLength = 11;
        public static readonly int MinLength = 11;
    }

    public static class Iban
    {
        public static readonly int MaxLength = 27;
        public static readonly int MinLength = 27;
    }
}
