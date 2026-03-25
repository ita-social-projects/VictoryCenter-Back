using VictoryCenter.BLL.DTOs.Admin.Localization.CompanyProfile;

namespace VictoryCenter.BLL.DTOs.Admin.CompanyProfileContacts;

public record UpdateCompanyProfileContactDto : BaseCompanyProfileContactsDto
{
    public List<UpdateCompanyProfileContactLocalizationDto> Localization { get; set; } = [];
}
