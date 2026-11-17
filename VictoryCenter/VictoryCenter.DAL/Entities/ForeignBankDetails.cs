using VictoryCenter.DAL.Entities.Attributes;
using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class ForeignBankDetails
{
    public long Id { get; set; }

    [Audited]
    public string Name { get; set; } = null!;

    [Audited]
    public string Receiver { get; set; } = null!;

    public string UkrainianIban { get; set; } = null!;

    public string Swift { get; set; } = null!;

    [Audited]
    public string Address { get; set; } = null!;

    [Audited]
    public BankCurrency Currency { get; set; }

    public List<CorrespondentBankDetails> CorrespondentBanks { get; set; } = [];
}
