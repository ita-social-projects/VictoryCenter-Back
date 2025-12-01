using VictoryCenter.DAL.Enums;

namespace VictoryCenter.DAL.Entities;

public class ForeignBankDetails
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Receiver { get; set; } = null!;
    public string UkrainianIban { get; set; } = null!;
    public string Swift { get; set; } = null!;
    public string Address { get; set; } = null!;
    public BankCurrency Currency { get; set; }
    public List<CorrespondentBankDetails> CorrespondentBanks { get; set; } = [];
}
