using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.TeamCategories.Create;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.TeamCategories;

public class CreateTeamCategoryValidator : AbstractValidator<CreateTeamCategoryCommand>
{
    public CreateTeamCategoryValidator()
    {
        RuleFor(command => command.CreateCategoryDto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Name"));
    }
}
