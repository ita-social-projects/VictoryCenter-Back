namespace VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;

public record CreateCorrespondentBankDetailsDto : BaseCorrespondentBankDetailsDto
{
    public long ForeignBankDetailsId { get; set; }
}
