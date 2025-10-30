using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.TeamCategories.Update;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.TeamCategories;

public class UpdateTeamCategoryValidator : AbstractValidator<UpdateTeamCategoryCommand>
{
    public UpdateTeamCategoryValidator()
    {
        RuleFor(command => command.UpdateCategoryDto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Name"));
    }
}
