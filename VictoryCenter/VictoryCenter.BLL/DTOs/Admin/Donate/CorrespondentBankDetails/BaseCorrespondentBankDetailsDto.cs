namespace VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;

public abstract record BaseCorrespondentBankDetailsDto
{
    public string Name { get; set; } = null!;
    public string Swift { get; set; } = null!;
    public string Account { get; set; } = null!;
    public string? Iban { get; set; }
}
