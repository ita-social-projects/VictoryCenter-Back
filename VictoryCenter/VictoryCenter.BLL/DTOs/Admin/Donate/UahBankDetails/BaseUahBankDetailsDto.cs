namespace VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;

public abstract record BaseUahBankDetailsDto
{
    public string Name { get; set; } = null!;
    public string Receiver { get; set; } = null!;
    public string Edrpou { get; set; } = null!;
    public string UkrainianIban { get; set; } = null!;
    public string PaymentPurpose { get; set; } = null!;
}
