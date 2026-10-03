using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;

public class CreateReportProgramExpendituresRecordDtoValidator
    : AbstractValidator<CreateReportProgramExpendituresRecordDto>
{
    public CreateReportProgramExpendituresRecordDtoValidator(
        BaseReportProgramExpendituresRecordValidator baseRecordValidator)
    {
        Include(baseRecordValidator);

        RuleFor(recordDto => recordDto.ReportingYear)
            .MustBeValidReportingYear(
                nameof(ReportProgramExpendituresRecordDto.ReportingYear),
                ReportProgramExpendituresRecordConstants.ReportingYearMinValue,
                ReportProgramExpendituresRecordConstants.ReportingYearMaxValue);
    }
}
