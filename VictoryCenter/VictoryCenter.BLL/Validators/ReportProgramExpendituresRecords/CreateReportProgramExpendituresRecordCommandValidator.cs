using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportProgramExpendituresRecords.Create;
using VictoryCenter.BLL.DTOs.Admin.ReportProgramExpendituresRecords;

namespace VictoryCenter.BLL.Validators.ReportProgramExpendituresRecords;

public class CreateReportProgramExpendituresRecordCommandValidator
    : AbstractValidator<CreateReportProgramExpendituresRecordCommand>
{
    public CreateReportProgramExpendituresRecordCommandValidator()
    {
        RuleFor(command => command.CreateReportProgramExpendituresRecordDto)
            .NotNull()
            .ChildRules(childValidator =>
            {
                childValidator.RuleFor(dto => dto.ReportingYear)
                    .MustBeValidReportingYear(nameof(ReportProgramExpendituresRecordDto.ReportingYear));

                childValidator.RuleFor(dto => dto.ProgramCategoryId)
                    .MustBeValidId(nameof(ReportProgramExpendituresRecordDto.ProgramCategoryId));

                childValidator.RuleFor(dto => dto.AmountUah)
                    .MustBeValidAmountOfMoney(nameof(ReportProgramExpendituresRecordDto.AmountUah));

                childValidator.RuleFor(dto => dto.AmountUsd)
                    .MustBeValidAmountOfMoney(nameof(ReportProgramExpendituresRecordDto.AmountUsd));
            });
    }
}
