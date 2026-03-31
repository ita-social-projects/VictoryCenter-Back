namespace VictoryCenter.BLL.DTOs.Admin.Localization.CompanyProfile;

public record BaseCompanyProfileRequisiteLocalizationDto
{
    public string? Recipient { get; init; }
    public string? Address { get; init; }
}
