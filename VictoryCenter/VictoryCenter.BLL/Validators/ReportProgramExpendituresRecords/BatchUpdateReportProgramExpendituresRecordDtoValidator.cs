using FluentValidation;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;

public class BatchUpdateReportProgramExpendituresRecordDtoValidator
    : AbstractValidator<BatchUpdateReportProgramExpendituresRecordDto>
{
    public BatchUpdateReportProgramExpendituresRecordDtoValidator(
        BaseReportProgramExpendituresRecordValidator baseRecordValidator)
    {
        Include(baseRecordValidator);

        RuleFor(e => e.Id)
            .MustBeValidId(nameof(BatchUpdateReportProgramExpendituresRecordDto.Id));
    }
}
