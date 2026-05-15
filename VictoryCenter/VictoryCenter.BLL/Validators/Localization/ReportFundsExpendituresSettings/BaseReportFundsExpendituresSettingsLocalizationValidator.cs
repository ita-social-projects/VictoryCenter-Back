using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresSettings;

namespace VictoryCenter.BLL.Validators.Localization.ReportFundsExpendituresSettings;

public class BaseReportFundsExpendituresSettingsLocalizationValidator
    : AbstractValidator<UpdateReportFundsExpendituresSettingsLocalizationDto>
{
    public BaseReportFundsExpendituresSettingsLocalizationValidator()
    {
        RuleFor(x => x.DisclaimerTitle)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateReportFundsExpendituresSettingsLocalizationDto.DisclaimerTitle)))
            .MinimumLength(ReportFundsExpendituresSettingsConstants.DisclaimerMinLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMinimumLengthOfNCharacters(
                nameof(UpdateReportFundsExpendituresSettingsLocalizationDto.DisclaimerTitle),
                ReportFundsExpendituresSettingsConstants.DisclaimerMinLength))
            .MaximumLength(ReportFundsExpendituresSettingsConstants.DisclaimerMaxLength)
            .WithMessage(ErrorMessagesConstants.PropertyMustHaveAMaximumLengthOfNCharacters(
                nameof(UpdateReportFundsExpendituresSettingsLocalizationDto.DisclaimerTitle),
                ReportFundsExpendituresSettingsConstants.DisclaimerMaxLength));
    }
}
