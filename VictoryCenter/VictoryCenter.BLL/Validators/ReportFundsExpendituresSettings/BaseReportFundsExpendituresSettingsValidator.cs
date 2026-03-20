using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.ReportFundsExpendituresSettings;

namespace VictoryCenter.BLL.Validators.ReportFundsExpendituresSettings;

public class BaseReportFundsExpendituresSettingsValidator : AbstractValidator<BaseReportFundsExpendituresSettingsDto>
{
    public BaseReportFundsExpendituresSettingsValidator()
    {
        RuleFor(dto => dto.DisclaimerTitle)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(ReportFundsExpendituresSettingsDto.DisclaimerTitle)))
            .MinimumLength(ReportFundsExpendituresSettingsConstants.DisclaimerMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(ReportFundsExpendituresSettingsDto.DisclaimerTitle),
                ReportFundsExpendituresSettingsConstants.DisclaimerMinLength))
            .MaximumLength(ReportFundsExpendituresSettingsConstants.DisclaimerMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(ReportFundsExpendituresSettingsDto.DisclaimerTitle),
                ReportFundsExpendituresSettingsConstants.DisclaimerMaxLength));

        RuleFor(dto => dto.ExchangeRate)
            .GreaterThan(ReportFundsExpendituresSettingsConstants.ExchangeRateMinValue)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(ReportFundsExpendituresSettingsDto.ExchangeRate)))
            .PrecisionScale(
                ReportFundsExpendituresSettingsConstants.ExchangeRatePrecision,
                ReportFundsExpendituresSettingsConstants.ExchangeRateScale,
                true)
            .WithMessage(ErrorMessagesConstants.PropertyMustBeInAValidFormat(
                nameof(ReportFundsExpendituresSettingsDto.ExchangeRate),
                ReportFundsExpendituresSettingsConstants.ExchangeRateFormat));
    }
}
