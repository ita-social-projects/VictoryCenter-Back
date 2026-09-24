using FluentValidation;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;

namespace VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;

public class UpdateReportFundsExpendituresRecordDtoValidator
    : AbstractValidator<UpdateReportFundsExpendituresRecordDto>
{
    public UpdateReportFundsExpendituresRecordDtoValidator(
        BaseReportFundsExpendituresRecordValidator baseRecordValidator)
    {
        Include(baseRecordValidator);
    }
}
