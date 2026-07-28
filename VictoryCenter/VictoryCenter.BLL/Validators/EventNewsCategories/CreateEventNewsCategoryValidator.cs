using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.EventNewsCategories.Create;

namespace VictoryCenter.BLL.Validators.EventNewsCategories;

public class CreateEventNewsCategoryValidator : AbstractValidator<CreateEventNewsCategoryCommand>
{
    public CreateEventNewsCategoryValidator()
    {
        RuleFor(command => command.Category.Name)
            .ValidEventNewsCategoryName();
    }
}
