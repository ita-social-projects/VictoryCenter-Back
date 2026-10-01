using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;
using VictoryCenter.BLL.Helpers;

namespace VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;

public class CreateReportFundsExpendituresRecordDtoValidator
    : AbstractValidator<CreateReportFundsExpendituresRecordDto>
{
    public CreateReportFundsExpendituresRecordDtoValidator(
        BaseReportFundsExpendituresRecordValidator baseRecordValidator,
        TimeProvider timeProvider)
    {
        var currentYear = timeProvider.GetUtcNow().Year;
        var minReportingYear = currentYear - 1;
        var maxReportingYear = currentYear + 1;

        Include(baseRecordValidator);

        RuleFor(recordDto => recordDto.Type)
            .IsInEnum()
            .WithMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(nameof(ReportFundsExpendituresRecordDto.Type)));

        RuleFor(recordDto => recordDto.ReportingYear)
            .MustBeValidReportingYear(
                nameof(ReportFundsExpendituresRecordDto.ReportingYear),
                minReportingYear,
                maxReportingYear);
    }
}
