using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class UahBankDetails : BankDetailsBase
{
    public required string Edrpou { get; set; }
    public required string PaymentPurpose { get; set; }

    public Currency Currency => Currency.Uah;
}
