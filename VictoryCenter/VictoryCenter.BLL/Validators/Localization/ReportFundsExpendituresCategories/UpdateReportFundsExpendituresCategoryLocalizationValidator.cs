using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresCategories.Update;

namespace VictoryCenter.BLL.Validators.Localization.ReportFundsExpendituresCategories;

public class UpdateReportFundsExpendituresCategoryLocalizationValidator
    : AbstractValidator<UpdateReportFundsExpendituresCategoryLocalizationCommand>
{
    public UpdateReportFundsExpendituresCategoryLocalizationValidator(
        BaseReportFundsExpendituresCategoryLocalizationValidator baseValidator)
    {
        RuleFor(x => x.UpdateReportFundsExpendituresCategoryLocalizationDto)
            .SetValidator(baseValidator);
    }
}
