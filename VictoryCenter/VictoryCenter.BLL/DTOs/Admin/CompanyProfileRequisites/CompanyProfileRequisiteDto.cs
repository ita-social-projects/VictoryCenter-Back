using VictoryCenter.BLL.DTOs.Admin.Localization.CompanyProfile;

namespace VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisites;

public record CompanyProfileRequisiteDto : BaseCompanyProfileRequisiteDto
{
    public List<CompanyProfileRequisiteLocalizationDto> Localizations { get; set; } = [];
}
