namespace VictoryCenter.BLL.DTOs.Public.Donate.CorrespondentBankDetails;

public record PublishedCorrespondentBankDetailsDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Swift { get; set; } = null!;
    public string Account { get; set; } = null!;
    public string? ForeignIban { get; set; }
    public long ForeignBankDetailsId { get; set; }
}
