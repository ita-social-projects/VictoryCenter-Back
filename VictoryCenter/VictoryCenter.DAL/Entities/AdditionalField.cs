namespace VictoryCenter.DAL.Entities;

public class AdditionalField
{
    public long Id { get; set; }
    public long? UahBankDetailsId { get; set; }
    public long? ForeignBankDetailsId { get; set; }
    public long? SupportMethodId { get; set; }
    public required string FieldName { get; set; }
    public required string FieldValue { get; set; }
    public DateTime CreatedAt { get; set; }

    public UahBankDetails? UahBankDetails { get; set; }
    public ForeignBankDetails? ForeignBankDetails { get; set; }
    public SupportMethod? SupportMethod { get; set; }
}
