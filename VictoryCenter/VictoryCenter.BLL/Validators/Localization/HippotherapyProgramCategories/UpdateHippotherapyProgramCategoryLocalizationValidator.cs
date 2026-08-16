using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.HippotherapyProgramCategories.Update;

namespace VictoryCenter.BLL.Validators.Localization.HippotherapyProgramCategories;

public class UpdateHippotherapyProgramCategoryLocalizationValidator
    : AbstractValidator<UpdateHippotherapyProgramCategoryLocalizationCommand>
{
    public UpdateHippotherapyProgramCategoryLocalizationValidator(
        BaseHippotherapyProgramCategoryLocalizationValidator baseValidator)
    {
        RuleFor(x => x.UpdateHippotherapyProgramCategoryLocalizationDto)
            .SetValidator(baseValidator);
    }
}
