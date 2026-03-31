namespace VictoryCenter.BLL.DTOs.Admin.CompanyProfileContacts;

public record BaseCompanyProfileContactsDto
{
    public string Phone { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string CorrespondenceEmail { get; set; } = null!;
    public string Motto { get; set; } = null!;
}
