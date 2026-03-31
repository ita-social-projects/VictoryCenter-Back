namespace VictoryCenter.BLL.DTOs.Admin.Localization.CompanyProfile;

public record UpdateCompanyProfileContactLocalizationDto : BaseCompanyProfileContactLocalizationDto
{
    public long LanguageId { get; init; }
}
