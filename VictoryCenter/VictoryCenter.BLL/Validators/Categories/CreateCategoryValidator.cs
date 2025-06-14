using FluentValidation;
using VictoryCenter.BLL.Commands.Categories.CreateCategory;

namespace VictoryCenter.BLL.Validators.Categories;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(command => command.createCategoryDto.Name)
            .NotEmpty()
            .WithMessage("Name can't be empty");
    }
}
