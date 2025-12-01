using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;

public record CreateForeignBankDetailsDto : BaseForeignBankDetailsDto
{
    public BankCurrency Currency { get; set; }
}
