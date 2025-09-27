namespace VictoryCenter.DAL.Entities;
public class CorrespondentBankDetails
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Swift { get; set; }
    public string Account { get; set; }
    public string? Iban { get; set; }
    public long ForeignBankDetailsId { get; set; }
    public ForeignBankDetails ForeignBankDetails { get; set; }
}
