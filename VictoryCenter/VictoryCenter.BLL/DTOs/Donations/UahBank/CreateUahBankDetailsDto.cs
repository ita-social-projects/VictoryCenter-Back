using VictoryCenter.BLL.DTOs.Donations.Common;

namespace VictoryCenter.BLL.DTOs.Donations.UahBank;

public class CreateUahBankDetailsDto : CreateBankDetailsBaseDto
{
    public string Edrpou { get; set; } = string.Empty;
    public string PaymentPurpose { get; set; } = string.Empty;
}
