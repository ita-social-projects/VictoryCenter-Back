using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
public record CreateForeignBankDetailsDto
{
    public string Name { get; set; } = null!;
    public string Receiver { get; set; } = null!;
    public string Iban { get; set; } = null!;
    public string Swift { get; set; } = null!;
    public string Address { get; set; } = null!;
    public BankCurrency Currency { get; set; }
    public List<CreateCorrespondentBankDetailsDto> CorrespondentBanks { get; set; } = [];
}
