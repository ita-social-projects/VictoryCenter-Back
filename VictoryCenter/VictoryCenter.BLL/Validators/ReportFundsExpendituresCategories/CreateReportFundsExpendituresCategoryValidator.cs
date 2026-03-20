using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.ReportFundsExpendituresCategories.Create;

namespace VictoryCenter.BLL.Validators.ReportFundsExpendituresCategories;

public class CreateReportFundsExpendituresCategoryValidator : AbstractValidator<CreateReportFundsExpendituresCategoryCommand>
{
    public CreateReportFundsExpendituresCategoryValidator(BaseReportFundsExpendituresCategoryValidator baseCategoryValidator)
    {
        RuleFor(command => command.CreateReportFundsExpendituresCategoryDto)
            .NotNull()
            .SetValidator(baseCategoryValidator);
    }
}
