using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class SupportOptions
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
    public BankCurrency Currency { get; set; }
}
