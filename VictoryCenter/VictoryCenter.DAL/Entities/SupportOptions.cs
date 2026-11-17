using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class SupportOptions
{
    public long Id { get; set; }

    [Audited]
    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;

    [Audited]
    public BankCurrency Currency { get; set; }
}
