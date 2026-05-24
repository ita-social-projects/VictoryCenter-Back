using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.ReportFundsExpendituresCategories.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.ReportFundsExpendituresCategories;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.ReportFundsExpendituresCategories;

public class CreateReportFundsExpendituresCategoryLocalizationValidator
    : AbstractValidator<CreateReportFundsExpendituresCategoryLocalizationCommand>
{
    public CreateReportFundsExpendituresCategoryLocalizationValidator(
        BaseReportFundsExpendituresCategoryLocalizationValidator baseValidator)
    {
        RuleFor(c => c.CreateReportFundsExpendituresCategoryLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateReportFundsExpendituresCategoryLocalizationDto>());
        RuleFor(c => c.CreateReportFundsExpendituresCategoryLocalizationDto)
            .SetValidator(baseValidator);
    }
}
