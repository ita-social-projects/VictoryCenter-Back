using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;

namespace VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;

public class CreateReportFundsExpendituresRecordValidator
    : AbstractValidator<CreateReportFundsExpendituresRecordCommand>
{
    public CreateReportFundsExpendituresRecordValidator(
        BaseReportFundsExpendituresRecordValidator baseRecordValidator)
    {
        RuleFor(command => command.CreateReportFundsExpendituresRecordDto)
            .NotNull()
            .SetValidator(baseRecordValidator);

        RuleFor(command => command.CreateReportFundsExpendituresRecordDto)
            .ChildRules(dto =>
        {
            dto.RuleFor(recordDto => recordDto.Type)
                .IsInEnum()
                .WithMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(ReportFundsExpendituresRecordDto.Type)));

            dto.RuleFor(recordDto => recordDto.ReportingYear)
                .GreaterThanOrEqualTo(ReportFundsExpendituresRecordConstants.ReportingYearMinValue)
                .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN(
                    nameof(ReportFundsExpendituresRecordDto.ReportingYear),
                    ReportFundsExpendituresRecordConstants.ReportingYearMinValue))
                .LessThanOrEqualTo(ReportFundsExpendituresRecordConstants.ReportingYearMaxValue)
                .WithMessage(ErrorMessagesConstants.PropertyMustBeLessThanOrEqualToN(
                    nameof(ReportFundsExpendituresRecordDto.ReportingYear),
                    ReportFundsExpendituresRecordConstants.ReportingYearMaxValue));
        });
    }
}
