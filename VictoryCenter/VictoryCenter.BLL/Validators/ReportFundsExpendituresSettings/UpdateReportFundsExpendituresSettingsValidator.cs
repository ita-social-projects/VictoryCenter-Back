using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresSettings.Update;

namespace VictoryCenter.BLL.Validators.ReportFundsExpendituresSettings;

public class UpdateReportFundsExpendituresSettingsValidator : AbstractValidator<UpdateReportFundsExpendituresSettingsCommand>
{
    public UpdateReportFundsExpendituresSettingsValidator(
        BaseReportFundsExpendituresSettingsValidator baseSettingsValidator)
    {
        RuleFor(command => command.UpdateReportFundsExpendituresSettingsDto)
            .NotNull()
            .SetValidator(baseSettingsValidator);
    }
}
