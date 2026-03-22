namespace VictoryCenter.DAL.Entities.Localization;

public class CompanyProfileContactLocalization : LocalizationBase<CompanyProfileContact>
{
    public string Address { get; set; } = null!;
    public string Motto { get; set; } = null!;
}
