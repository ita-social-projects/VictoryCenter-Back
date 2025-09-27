namespace VictoryCenter.DAL.Entities;
public class ForeignBankDetails
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Receiver { get; set; }
    public string Iban { get; set; }
    public string Swift { get; set; }
    public string Address { get; set; }
    public List<CorrespondentBankDetails> CorrespondentBanks { get; set; }
}
