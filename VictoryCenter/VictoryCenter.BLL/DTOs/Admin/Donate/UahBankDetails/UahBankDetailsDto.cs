namespace VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;
public record UahBankDetailsDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Receiver { get; set; }
    public string Edrpou { get; set; }
    public string Iban { get; set; }
    public string PaymentPurpose { get; set; }
}
