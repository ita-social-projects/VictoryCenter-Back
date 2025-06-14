using FluentValidation;
using VictoryCenter.BLL.Commands.Categories.UpdateCategory;

namespace VictoryCenter.BLL.Validators.Categories;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(command => command.updateCategoryDto.Name)
            .NotEmpty()
            .WithMessage("Name can't be empty");
    }
}
