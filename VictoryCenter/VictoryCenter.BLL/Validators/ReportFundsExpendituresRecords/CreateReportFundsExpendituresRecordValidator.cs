using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Create;

namespace VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;

public class CreateReportFundsExpendituresRecordValidator
    : AbstractValidator<CreateReportFundsExpendituresRecordCommand>
{
    public CreateReportFundsExpendituresRecordValidator(
        CreateReportFundsExpendituresRecordDtoValidator recordDtoValidator)
    {
        RuleFor(command => command.CreateReportFundsExpendituresRecordDto)
            .NotNull()
            .SetValidator(recordDtoValidator);
    }
}
