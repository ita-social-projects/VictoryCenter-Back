namespace VictoryCenter.BLL.DTOs.Donations.Common;

public abstract class CreateBankDetailsBaseDto
{
    public string BankName { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public List<CreateAdditionalFieldDto> AdditionalFields { get; set; } = new();
}
