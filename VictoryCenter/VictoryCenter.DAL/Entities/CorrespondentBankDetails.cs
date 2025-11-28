namespace VictoryCenter.DAL.Entities;

public class CorrespondentBankDetails
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Swift { get; set; } = null!;
    public string? Account { get; set; }
    public string? ForeignIban { get; set; }
    public long ForeignBankDetailsId { get; set; }
    public ForeignBankDetails ForeignBankDetails { get; set; } = null!;
}
