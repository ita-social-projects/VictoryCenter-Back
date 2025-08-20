using VictoryCenter.BLL.DTOs.Donations.SupportMethod;

namespace VictoryCenter.BLL.DTOs.Donations.Common;

public class DonationsByCurrencyDto
{
    public required BankDetailsDto BankDetails { get; set; }
    public required SupportMethodDto OtherMethod { get; set; }
}
