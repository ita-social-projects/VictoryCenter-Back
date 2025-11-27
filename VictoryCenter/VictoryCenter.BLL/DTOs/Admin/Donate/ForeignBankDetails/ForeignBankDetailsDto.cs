using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;

public record ForeignBankDetailsDto : BaseForeignBankDetailsDto
{
    public long Id { get; set; }
    public BankCurrency Currency { get; set; }
    public List<CorrespondentBankDetailsDto> CorrespondentBanks { get; set; } = [];
}
