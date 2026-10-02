using FluentValidation;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;
public class BatchUpdateReportFundsExpendituresRecordDtoValidator
    : AbstractValidator<BatchUpdateReportFundsExpendituresRecordDto>
{
    public BatchUpdateReportFundsExpendituresRecordDtoValidator(
        BaseReportFundsExpendituresRecordValidator baseRecordValidator)
    {
        Include(baseRecordValidator);

        RuleFor(e => e.Id)
            .MustBeValidId(nameof(BatchUpdateReportFundsExpendituresRecordDto.Id));
    }
}
