using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Update;

namespace VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;

public class UpdateReportFundsExpendituresRecordValidator
    : AbstractValidator<UpdateReportFundsExpendituresRecordCommand>
{
    public UpdateReportFundsExpendituresRecordValidator(
        UpdateReportFundsExpendituresRecordDtoValidator recordDtoValidator)
    {
        RuleFor(command => command.UpdateReportFundsExpendituresRecordDto)
            .NotNull()
            .SetValidator(recordDtoValidator);
    }
}
