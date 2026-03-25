using VictoryCenter.BLL.DTOs.Admin.Localization.CompanyProfile;

namespace VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisites;

public record UpdateCompanyProfileRequisiteDto : BaseCompanyProfileRequisiteDto
{
    public List<UpdateCompanyProfileRequisiteLocalizationDto> Localization { get; set; } = [];
}
