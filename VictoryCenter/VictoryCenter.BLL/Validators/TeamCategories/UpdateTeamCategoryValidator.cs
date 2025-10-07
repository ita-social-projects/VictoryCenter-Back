using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.TeamCategories.Update;

namespace VictoryCenter.BLL.Validators.TeamCategories;

public class UpdateTeamCategoryValidator : AbstractValidator<UpdateTeamCategoryCommand>
{
    public UpdateTeamCategoryValidator(BaseTeamCategoryValidator baseValidator)
    {
        RuleFor(x => x.UpdateTeamCategoryDto).SetValidator(baseValidator);
    }
}
