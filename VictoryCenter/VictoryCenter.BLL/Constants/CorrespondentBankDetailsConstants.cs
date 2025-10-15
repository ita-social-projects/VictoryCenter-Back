namespace VictoryCenter.BLL.Constants;
public static class CorrespondentBankDetailsConstants
{
    public static class Swift
    {
        public static readonly int MaxLength = 11;
        public static readonly int MinLength = 8;
    }

    public static class Iban
    {
        public static readonly int MaxLength = 34;
        public static readonly int MinLength = 15;
    }
}
