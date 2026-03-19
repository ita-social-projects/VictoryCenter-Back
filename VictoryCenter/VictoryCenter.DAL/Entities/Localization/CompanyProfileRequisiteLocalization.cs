namespace VictoryCenter.DAL.Entities.Localization;

public class CompanyProfileRequisiteLocalization : LocalizationBase<CompanyProfileRequisite>
{
    public string? Recipient { get; set; } = null!;
    public string? Address { get; set; } = null!;
}
