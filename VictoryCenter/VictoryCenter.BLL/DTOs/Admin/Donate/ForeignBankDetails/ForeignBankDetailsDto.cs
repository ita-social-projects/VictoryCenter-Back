using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;

namespace VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
public record ForeignBankDetailsDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Receiver { get; set; }
    public string Iban { get; set; }
    public string Swift { get; set; }
    public string Address { get; set; }
    public List<CorrespondentBankDetailsDto> CorrespondentBanks { get; set; } = [];
}
