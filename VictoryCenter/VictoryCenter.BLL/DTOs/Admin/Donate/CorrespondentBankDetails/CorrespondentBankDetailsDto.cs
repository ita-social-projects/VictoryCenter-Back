namespace VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;

public record CorrespondentBankDetailsDto : BaseCorrespondentBankDetailsDto
{
    public long Id { get; set; }
    public long ForeignBankDetailsId { get; set; }
}
