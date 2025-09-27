namespace VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
public record CorrespondentBankDetailsDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Swift { get; set; }
    public string Account { get; set; }
    public string? Iban { get; set; }
    public long ForeignBankDetailsId { get; set; }
}
