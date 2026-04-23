using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresRecords.Create;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;

namespace VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;

public class CreateReportFundsExpendituresRecordValidator
    : AbstractValidator<CreateReportFundsExpendituresRecordCommand>
{
    public CreateReportFundsExpendituresRecordValidator(
        BaseReportFundsExpendituresRecordValidator baseRecordValidator,
        TimeProvider timeProvider)
    {
        var currentYear = timeProvider.GetUtcNow().Year;
        var minReportingYear = currentYear - 1;
        var maxReportingYear = currentYear + 1;

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
                .GreaterThanOrEqualTo(minReportingYear)
                .WithMessage(ErrorMessagesConstants.PropertyMustBeGreaterThanOrEqualToN(
                    nameof(ReportFundsExpendituresRecordDto.ReportingYear),
                    minReportingYear))
                .LessThanOrEqualTo(maxReportingYear)
                .WithMessage(ErrorMessagesConstants.PropertyMustBeLessThanOrEqualToN(
                    nameof(ReportFundsExpendituresRecordDto.ReportingYear),
                    maxReportingYear));
        });
    }
}
