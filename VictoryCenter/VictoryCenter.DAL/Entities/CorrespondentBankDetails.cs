using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities;

[Auditable]
public class CorrespondentBankDetails
{
    public long Id { get; set; }

    [Audited]
    public string Name { get; set; } = null!;

    public string Swift { get; set; } = null!;

    public string? Account { get; set; }

    public string? ForeignIban { get; set; }

    [Audited]
    public long ForeignBankDetailsId { get; set; }

    public ForeignBankDetails ForeignBankDetails { get; set; } = null!;
}
