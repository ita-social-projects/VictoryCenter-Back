using FluentValidation;

namespace VictoryCenter.BLL.Commands.Categories.CreateCategory;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(command => command.createCategoryDto.Name)
            .NotNull()
            .WithMessage("Name is required");
        
        RuleFor(command => command.createCategoryDto.Name)
            .NotEmpty()
            .WithMessage("Name can't be empty");
    }
}
