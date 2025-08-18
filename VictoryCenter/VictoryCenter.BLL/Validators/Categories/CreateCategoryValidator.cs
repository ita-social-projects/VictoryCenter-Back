using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Categories.Create;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.Categories;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(command => command.createCategoryDto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Name"));
    }
}
