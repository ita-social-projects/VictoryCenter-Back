using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.TeamCategories.Create;

namespace VictoryCenter.BLL.Validators.TeamCategories;

public class CreateTeamCategoryValidator : AbstractValidator<CreateTeamCategoryCommand>
{
    public CreateTeamCategoryValidator(BaseTeamCategoryValidator baseValidator)
    {
        RuleFor(x => x.CreateTeamCategoryDto).NotNull().SetValidator(baseValidator);
    }
}
