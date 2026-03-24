using VictoryCenter.BLL.DTOs.Admin.Localization.CompanyProfile;

namespace VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisite;

public record CreateCompanyProfileRequisiteDto : BaseCompanyProfileRequisiteDto
{
    public List<CreateCompanyProfileRequisiteLocalizationDto> Localization { get; set; } = [];
}
