namespace VictoryCenter.DAL.Entities;

public class BankDetailsBase
{
    public long Id { get; set; }
    public required string BankName { get; set; }
    public required string Recipient { get; set; }
    public required string Iban { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<AdditionalField> AdditionalFields { get; set; } = new List<AdditionalField>();
}
