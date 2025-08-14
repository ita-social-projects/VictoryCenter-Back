using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Categories.Update;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.Categories;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(command => command.UpdateCategoryDto.Name)
            .NotEmpty()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired("Name"));
    }
}
