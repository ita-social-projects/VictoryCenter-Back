using VictoryCenter.BLL.DTOs.Admin.Localization.CompanyProfile;

namespace VictoryCenter.BLL.DTOs.Admin.CompanyProfileContacts;

public record CompanyProfileContactsDto : BaseCompanyProfileContactsDto
{
    public List<CompanyProfileContactLocalizationDto> Localizations { get; set; } = [];
}
