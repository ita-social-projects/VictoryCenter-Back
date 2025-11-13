namespace VictoryCenter.BLL.Constants;

public static class ForeignBankDetailsConstants
{
    public static readonly string OnlyUsdOrEurMessage = "Currency must be USD or EUR";
    public static readonly string IbanMustStartWithUaFollowedByDigits = "IBAN must start with UA followed by digits only";
    public static readonly string UahIbanExpression = @"^UA\d+$";

    public static readonly int NameMaxLength = 200;
    public static readonly int ReceiverMaxLength = 200;
    public static readonly int AddressMaxLength = 200;
    public static class Swift
    {
        public static readonly int MaxLength = 11;
        public static readonly int MinLength = 8;
    }

    public static class Iban
    {
        public static readonly int MaxLength = 29;
        public static readonly int MinLength = 29;
    }
}
