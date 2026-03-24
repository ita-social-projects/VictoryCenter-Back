using VictoryCenter.BLL.DTOs.Admin.Localization.CompanyProfile;

namespace VictoryCenter.BLL.DTOs.Admin.CompanyProfileContacts;

public record CreateCompanyProfileContactsDto : BaseCompanyProfileContactsDto
{
    public List<CreateCompanyProfileContactLocalizationDto> Localization { get; set; } = [];
}
