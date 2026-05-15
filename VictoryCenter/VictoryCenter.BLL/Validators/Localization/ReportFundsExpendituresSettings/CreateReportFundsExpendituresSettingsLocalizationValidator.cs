using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresSettings.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresSettings;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.ReportFundsExpendituresSettings;

public class CreateReportFundsExpendituresSettingsLocalizationValidator
    : AbstractValidator<CreateReportFundsExpendituresSettingsLocalizationCommand>
{
    public CreateReportFundsExpendituresSettingsLocalizationValidator(
        BaseReportFundsExpendituresSettingsLocalizationValidator baseValidator)
    {
        RuleFor(c => c.CreateReportFundsExpendituresSettingsLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateReportFundsExpendituresSettingsLocalizationDto>());
        RuleFor(c => c.CreateReportFundsExpendituresSettingsLocalizationDto)
            .SetValidator(baseValidator);
    }
}
