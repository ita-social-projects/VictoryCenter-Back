using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;

public record CreateSupportOptionsDto : BaseSupportOptionsDto
{
    public BankCurrency Currency { get; set; }
}
