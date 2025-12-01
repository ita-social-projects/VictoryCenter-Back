using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;

public record SupportOptionsDto : BaseSupportOptionsDto
{
    public long Id { get; set; }
    public BankCurrency Currency { get; set; }
}
