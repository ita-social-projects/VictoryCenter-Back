using VictoryCenter.BLL.DTOs.Donations.SupportMethod;

namespace VictoryCenter.BLL.DTOs.Donations.Common;

public class CurrencyDonationsDto
{
    public BankDetailsDto? BankDetailsDto { get; set; }
    public SupportMethodDto? SupportMethodDto { get; set; }
}
