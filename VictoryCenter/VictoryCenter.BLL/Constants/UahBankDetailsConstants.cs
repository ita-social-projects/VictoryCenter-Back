namespace VictoryCenter.BLL.Constants;

public static class UahBankDetailsConstants
{
    public static readonly string UahIbanExpression = @"^UA\d+$";
    public static readonly string UkrainianIbanMustStartWithUaFollowedByDigits = "IBAN must start with UA followed by digits only";

    public static readonly int NameMaxLength = 200;
    public static readonly int ReceiverMaxLength = 200;
    public static readonly int PaymentPurposeMaxLength = 500;
    public static class Edrpou
    {
        public static readonly int MaxLength = 8;
        public static readonly int MinLength = 8;
    }

    public static class UkrainianIban
    {
        public static readonly int MaxLength = 29;
        public static readonly int MinLength = 29;
    }
}
