using FluentValidation;

namespace VictoryCenter.BLL.Commands.Categories.UpdateCategory;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(command => command.updateCategoryDto.Name)
            .NotNull()
            .WithMessage("Name is required");
        
        RuleFor(command => command.updateCategoryDto.Name)
            .NotEmpty()
            .WithMessage("Name can't be empty");
    }
}
