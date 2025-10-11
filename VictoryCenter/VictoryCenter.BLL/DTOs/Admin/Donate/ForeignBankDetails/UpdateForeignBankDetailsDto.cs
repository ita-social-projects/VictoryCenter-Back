using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;

namespace VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;
public record UpdateForeignBankDetailsDto
{
    public string Name { get; set; } = null!;
    public string Receiver { get; set; } = null!;
    public string Iban { get; set; } = null!;
    public string Swift { get; set; } = null!;
    public string Address { get; set; } = null!;
    public List<UpdateCorrespondentBankDetailsDto> CorrespondentBanks { get; set; } = [];
}
