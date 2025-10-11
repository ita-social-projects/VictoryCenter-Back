namespace VictoryCenter.BLL.Constants;
public static class UahBankDetailsConstants
{
    public static readonly string OnlyDigitsMessage = "only digits allowed";
    public static readonly string OnlyDigits = "^[0-9]+$";

    public static class Edrpou
    {
        public static readonly int MaxLength = 8;
        public static readonly int MinLength = 8;
    }

    public static class Iban
    {
        public static readonly int MaxLength = 27;
        public static readonly int MinLength = 27;
    }
}
