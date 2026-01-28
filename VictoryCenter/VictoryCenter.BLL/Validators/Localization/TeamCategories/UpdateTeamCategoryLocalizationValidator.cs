using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.TeamCategories.Update;

namespace VictoryCenter.BLL.Validators.Localization.TeamCategories;
public class UpdateTeamCategoryLocalizationValidator : AbstractValidator<UpdateTeamCategoryLocalizationCommand>
{
    public UpdateTeamCategoryLocalizationValidator(BaseTeamCategoryLocalizationValidator baseTeamCategoryLocalizationValidator)
    {
        RuleFor(x => x.UpdateTeamCategoryLocalizationDto)
            .SetValidator(baseTeamCategoryLocalizationValidator);
    }
}
