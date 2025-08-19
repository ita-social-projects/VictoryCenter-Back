namespace VictoryCenter.BLL.DTOs.Donations.ForeignBank;

public class CorrespondentBankDto
{
    public long Id { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string Account { get; set; } = string.Empty;
    public string SwiftCode { get; set; } = string.Empty;
}
