using VictoryCenter.BLL.DTOs.Admin.Localization.CompanyProfile;

namespace VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisites;

public record CreateCompanyProfileRequisiteDto : BaseCompanyProfileRequisiteDto
{
    public List<CreateCompanyProfileRequisiteLocalizationDto> Localization { get; set; } = [];
}
