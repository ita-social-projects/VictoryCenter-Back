using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;

namespace VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
public record UpdateForeignBankDetailsDto
{
    public string Name { get; set; }
    public string Receiver { get; set; }
    public string Iban { get; set; }
    public string Swift { get; set; }
    public string Address { get; set; }
    public List<UpdateCorrespondentBankDetailsDto> CorrespondentBanks { get; set; } = [];
}
