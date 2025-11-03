using VictoryCenter.BLL.DTOs.Public.Donate.CorrespondentBankDetails;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Public.Donate.ForeignBankDetails;

public record PublishedForeignBankDetailsDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Receiver { get; set; } = null!;
    public string Iban { get; set; } = null!;
    public string Swift { get; set; } = null!;
    public string Address { get; set; } = null!;
    public BankCurrency Currency { get; set; }
    public List<PublishedCorrespondentBankDetailsDto> CorrespondentBanks { get; set; } = [];
}
