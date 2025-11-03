using VictoryCenter.DAL.Enums;

namespace VictoryCenter.BLL.DTOs.Public.Donate.SupportOptions;

public record PublishedSupportOptionsDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
    public BankCurrency Currency { get; set; }
}
