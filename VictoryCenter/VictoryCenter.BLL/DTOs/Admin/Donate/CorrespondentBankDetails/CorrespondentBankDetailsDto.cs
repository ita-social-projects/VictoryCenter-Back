namespace VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;
public record CorrespondentBankDetailsDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Swift { get; set; } = null!;
    public string Account { get; set; } = null!;
    public string? Iban { get; set; }
    public long ForeignBankDetailsId { get; set; }
}
