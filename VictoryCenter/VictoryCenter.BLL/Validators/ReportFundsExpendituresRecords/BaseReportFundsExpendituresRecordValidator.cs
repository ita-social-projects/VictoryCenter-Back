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

        RuleFor(dto => dto.Amount)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(
                nameof(BaseReportFundsExpendituresRecordDto.Amount)))
            .GreaterThanOrEqualTo(ReportFundsExpendituresRecordConstants.ZeroAmount)
            .WithMessage(ErrorMessagesConstants.SumMustNotBeNegative(
                nameof(BaseReportFundsExpendituresRecordDto.Amount)))
            .NotEqual(ReportFundsExpendituresRecordConstants.ZeroAmount)
            .WithMessage(ErrorMessagesConstants.SumNotEqualTo(
                nameof(BaseReportFundsExpendituresRecordDto.Amount),
                ReportFundsExpendituresRecordConstants.ZeroAmount))
            .PrecisionScale(
                ReportFundsExpendituresRecordConstants.AmountPrecision,
                ReportFundsExpendituresRecordConstants.AmountScale,
                true)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(BaseReportFundsExpendituresRecordDto.Amount),
                ReportFundsExpendituresRecordConstants.AmountFormat));

        RuleFor(dto => dto.Currency)
            .IsInEnum()
            .WithMessage(ErrorMessagesConstants.PropertyMustBeValidEnum(
                nameof(BaseReportFundsExpendituresRecordDto.Currency)));
    }
}
