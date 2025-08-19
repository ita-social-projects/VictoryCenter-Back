using VictoryCenter.BLL.DTOs.Donations.ForeignBank;

namespace VictoryCenter.BLL.DTOs.Donations.Common;

public class BankDetailsDto
{
    public long Id { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public string SwiftCode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Edrpou { get; set; } = string.Empty;
    public string PaymentPurpose { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<AdditionalFieldDto> AdditionalFields { get; set; } = new List<AdditionalFieldDto>();
    public List<CorrespondentBankDto> CorrespondentBanks { get; set; } = new List<CorrespondentBankDto>();
}
