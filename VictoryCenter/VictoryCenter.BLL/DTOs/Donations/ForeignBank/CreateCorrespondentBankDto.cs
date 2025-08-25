namespace VictoryCenter.BLL.DTOs.Donations.ForeignBank;

public class CreateCorrespondentBankDto
{
    public string BankName { get; set; } = string.Empty;
    public string Account { get; set; } = string.Empty;
    public string SwiftCode { get; set; } = string.Empty;
}
