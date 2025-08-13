namespace VictoryCenter.DAL.Entities;

public class CorrespondentBank
{
    public long Id { get; set; }
    public long ForeignBankDetailsId { get; set; }
    public required string BankName { get; set; }
    public required string Account { get; set; }
    public required string SwiftCode { get; set; }
    public DateTime CreatedAt { get; set; }

    public required ForeignBankDetails ForeignBankDetails { get; set; }
}
