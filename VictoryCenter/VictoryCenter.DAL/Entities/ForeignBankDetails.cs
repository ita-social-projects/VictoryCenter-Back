using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class ForeignBankDetails : BankDetailsBase
{
    public Currency Currency { get; set; }
    public required string SwiftCode { get; set; }
    public required string Address { get; set; }

    public ICollection<CorrespondentBank> CorrespondentBanks { get; set; } = new List<CorrespondentBank>();
}
