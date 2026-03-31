namespace VictoryCenter.BLL.DTOs.Admin.Localization.CompanyProfile;

public record UpdateCompanyProfileRequisiteLocalizationDto : BaseCompanyProfileRequisiteLocalizationDto
{
    public long LanguageId { get; init; }
}
