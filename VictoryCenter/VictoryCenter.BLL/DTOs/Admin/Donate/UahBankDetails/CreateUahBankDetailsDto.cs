namespace VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;
public record CreateUahBankDetailsDto
{
    public string Name { get; init; }
    public string Receiver { get; init; }
    public string Edrpou { get; init; }
    public string Iban { get; init; }
    public string PaymentPurpose { get; init; }
}
