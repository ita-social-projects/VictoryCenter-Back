using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Admin.Donate.SupportOptions;

public record CreateSupportOptionsDto : ISupportOptions
{
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
    public BankCurrency Currency { get; set; }
}
