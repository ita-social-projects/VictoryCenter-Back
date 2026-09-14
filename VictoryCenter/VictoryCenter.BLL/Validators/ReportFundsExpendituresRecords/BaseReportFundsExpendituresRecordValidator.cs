using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresRecords;

namespace VictoryCenter.BLL.Validators.ReportFundsExpendituresRecords;

public class BaseReportFundsExpendituresRecordValidator : AbstractValidator<BaseReportFundsExpendituresRecordDto>
{
    public BaseReportFundsExpendituresRecordValidator()
    {
        RuleFor(dto => dto.CategoryId)
            .GreaterThan(ReportFundsExpendituresCategoryConstants.ZeroCategoryId)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ReportFundsExpendituresRecordDto.CategoryId)));

        RuleFor(dto => dto.AmountUah)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(ReportFundsExpendituresRecordDto.AmountUah)))
            .GreaterThanOrEqualTo(ReportFundsExpendituresRecordConstants.ZeroAmount)
            .WithMessage(ErrorMessagesConstants.SumMustNotBeNegative(
                nameof(ReportFundsExpendituresRecordDto.AmountUah)))
            .NotEqual(ReportFundsExpendituresRecordConstants.ZeroAmount)
            .WithMessage(ErrorMessagesConstants.SumNotEqualTo(
                nameof(ReportFundsExpendituresRecordDto.AmountUah),
                ReportFundsExpendituresRecordConstants.ZeroAmount))
            .PrecisionScale(
                ReportFundsExpendituresRecordConstants.AmountPrecision,
                ReportFundsExpendituresRecordConstants.AmountScale,
                true)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(ReportFundsExpendituresRecordDto.AmountUah),
                ReportFundsExpendituresRecordConstants.AmountFormat));

        RuleFor(dto => dto.AmountUsd)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(ReportFundsExpendituresRecordDto.AmountUsd)))
            .GreaterThanOrEqualTo(ReportFundsExpendituresRecordConstants.ZeroAmount)
            .WithMessage(ErrorMessagesConstants.SumMustNotBeNegative(
                nameof(ReportFundsExpendituresRecordDto.AmountUsd)))
            .NotEqual(ReportFundsExpendituresRecordConstants.ZeroAmount)
            .WithMessage(ErrorMessagesConstants.SumNotEqualTo(
                nameof(ReportFundsExpendituresRecordDto.AmountUsd),
                ReportFundsExpendituresRecordConstants.ZeroAmount))
            .PrecisionScale(
                ReportFundsExpendituresRecordConstants.AmountPrecision,
                ReportFundsExpendituresRecordConstants.AmountScale,
                true)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(ReportFundsExpendituresRecordDto.AmountUsd),
                ReportFundsExpendituresRecordConstants.AmountFormat));
    }
}
