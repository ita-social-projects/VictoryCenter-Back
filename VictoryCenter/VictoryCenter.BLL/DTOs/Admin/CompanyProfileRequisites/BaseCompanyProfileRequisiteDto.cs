namespace VictoryCenter.BLL.DTOs.Admin.CompanyProfileRequisite;

public record BaseCompanyProfileRequisiteDto
{
    public string Recipient { get; set; } = null!;
    public string Edrpou { get; set; } = null!;
    public string Address { get; set; } = null!;
}
