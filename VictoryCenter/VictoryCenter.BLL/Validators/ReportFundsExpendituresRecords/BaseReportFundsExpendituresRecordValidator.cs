using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;

namespace VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;

public class BaseReportFundsExpendituresRecordValidator : AbstractValidator<BaseReportFundsExpendituresRecordDto>
{
    public BaseReportFundsExpendituresRecordValidator()
    {
        RuleFor(dto => dto.CategoryId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ReportFundsExpendituresRecordDto.CategoryId)));

        RuleFor(dto => dto.AmountUah)
            .GreaterThan(ReportFundsExpendituresRecordConstants.AmountMinValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ReportFundsExpendituresRecordDto.AmountUah)))
            .PrecisionScale(
                ReportFundsExpendituresRecordConstants.AmountPrecision,
                ReportFundsExpendituresRecordConstants.AmountScale,
                true)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(ReportFundsExpendituresRecordDto.AmountUah),
                ReportFundsExpendituresRecordConstants.AmountFormat));

        RuleFor(dto => dto.AmountUsd)
            .GreaterThan(ReportFundsExpendituresRecordConstants.AmountMinValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ReportFundsExpendituresRecordDto.AmountUsd)))
            .PrecisionScale(
                ReportFundsExpendituresRecordConstants.AmountPrecision,
                ReportFundsExpendituresRecordConstants.AmountScale,
                true)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(ReportFundsExpendituresRecordDto.AmountUsd),
                ReportFundsExpendituresRecordConstants.AmountFormat));
    }
}
