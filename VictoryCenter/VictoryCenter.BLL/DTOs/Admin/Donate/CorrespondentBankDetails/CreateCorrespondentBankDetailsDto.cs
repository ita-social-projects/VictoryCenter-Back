namespace VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
public record CreateCorrespondentBankDetailsDto
{
    public string Name { get; set; }
    public string Swift { get; set; }
    public string Account { get; set; }
    public string? Iban { get; set; }
}
