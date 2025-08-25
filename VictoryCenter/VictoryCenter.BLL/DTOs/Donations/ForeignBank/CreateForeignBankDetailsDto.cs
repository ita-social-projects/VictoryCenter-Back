using VictoryCenter.BLL.DTOs.Donations.Common;

namespace VictoryCenter.BLL.DTOs.Donations.ForeignBank;

public class CreateForeignBankDetailsDto : CreateBankDetailsBaseDto
{
    public string Currency { get; set; } = string.Empty;
    public string SwiftCode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public List<CreateCorrespondentBankDto> CorrespondentBanks { get; set; } = new();
}
