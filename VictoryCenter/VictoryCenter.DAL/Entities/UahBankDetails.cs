using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class UahBankDetails
{
    public long Id { get; set; }

    [Audited]
    public string Name { get; set; } = null!;

    [Audited]
    public string Receiver { get; set; } = null!;

    public string Edrpou { get; set; } = null!;

    public string UkrainianIban { get; set; } = null!;

    [Audited]
    public string PaymentPurpose { get; set; } = null!;
}
