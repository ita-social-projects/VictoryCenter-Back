using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Create;

namespace VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;

public class CreateReportProgramExpendituresRecordCommandValidator
    : AbstractValidator<CreateReportProgramExpendituresRecordCommand>
{
    public CreateReportProgramExpendituresRecordCommandValidator(
        CreateReportProgramExpendituresRecordDtoValidator recordDtoValidator)
    {
        RuleFor(command => command.CreateReportProgramExpendituresRecordDto)
            .NotNull()
            .SetValidator(recordDtoValidator);
    }
}
