using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class CompanyProfileRequisiteLocalization : LocalizationBase<CompanyProfileRequisite>
{
    [Audited]
    public string? Recipient { get; set; } = null!;

    [Audited]
    public string? Address { get; set; } = null!;
}
