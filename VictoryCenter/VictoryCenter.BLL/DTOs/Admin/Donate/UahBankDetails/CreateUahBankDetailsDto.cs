namespace VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;
public record CreateUahBankDetailsDto
{
    public string Name { get; init; } = null!;
    public string Receiver { get; init; } = null!;
    public string Edrpou { get; init; } = null!;
    public string Iban { get; init; } = null!;
    public string PaymentPurpose { get; init; } = null!;
}
