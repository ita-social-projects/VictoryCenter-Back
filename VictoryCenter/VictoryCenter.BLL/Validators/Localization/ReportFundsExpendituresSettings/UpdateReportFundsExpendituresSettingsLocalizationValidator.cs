using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresSettings.Update;

namespace VictoryCenter.BLL.Validators.Localization.ReportFundsExpendituresSettings;

public class UpdateReportFundsExpendituresSettingsLocalizationValidator
    : AbstractValidator<UpdateReportFundsExpendituresSettingsLocalizationCommand>
{
    public UpdateReportFundsExpendituresSettingsLocalizationValidator(
        BaseReportFundsExpendituresSettingsLocalizationValidator baseValidator)
    {
        RuleFor(x => x.UpdateReportFundsExpendituresSettingsLocalizationDto)
            .SetValidator(baseValidator);
    }
}
