using VictoryCenter.BLL.DTOs.Admin.Localization.CompanyProfile;

namespace VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisite;

public record CompanyProfileRequisiteDto : BaseCompanyProfileRequisiteDto
{
    public List<CompanyProfileRequisiteLocalizationDto> Localizations { get; set; } = [];
}
