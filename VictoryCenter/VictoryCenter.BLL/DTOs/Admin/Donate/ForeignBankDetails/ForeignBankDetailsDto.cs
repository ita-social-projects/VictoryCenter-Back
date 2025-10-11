using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
public record ForeignBankDetailsDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Receiver { get; set; } = null!;
    public string Iban { get; set; } = null!;
    public string Swift { get; set; } = null!;
    public string Address { get; set; } = null!;
    public BankCurrency Currency { get; set; }
    public List<CorrespondentBankDetailsDto> CorrespondentBanks { get; set; } = [];
}
