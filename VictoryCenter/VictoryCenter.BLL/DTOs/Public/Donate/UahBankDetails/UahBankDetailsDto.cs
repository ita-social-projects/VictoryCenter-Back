namespace VictoryCenter.BLL.DTOs.Public.Donate.UahBankDetails;
public record UahBankDetailsDto
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Receiver { get; set; } = null!;
    public string Edrpou { get; set; } = null!;
    public string Iban { get; set; } = null!;
    public string PaymentPurpose { get; set; } = null!;
}
