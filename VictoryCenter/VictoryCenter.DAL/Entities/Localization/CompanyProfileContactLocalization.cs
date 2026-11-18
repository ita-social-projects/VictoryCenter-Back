using VictoryCenter.DAL.Entities.Attributes;

namespace VictoryCenter.DAL.Entities.Localization;

public class CompanyProfileContactLocalization : LocalizationBase<CompanyProfileContact>
{
    [Audited]
    public string Address { get; set; } = null!;

    [Audited]
    public string Motto { get; set; } = null!;
}
