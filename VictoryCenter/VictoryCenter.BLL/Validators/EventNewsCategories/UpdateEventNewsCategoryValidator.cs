using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.EventNewsCategories.Update;

namespace VictoryCenter.BLL.Validators.EventNewsCategories;

public class UpdateEventNewsCategoryValidator : AbstractValidator<UpdateEventNewsCategoryCommand>
{
    public UpdateEventNewsCategoryValidator()
    {
        RuleFor(command => command.Category.Name)
            .ValidEventNewsCategoryName();
    }
}
