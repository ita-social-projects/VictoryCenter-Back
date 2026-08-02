using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNewsCategories.Create;
using VictoryCenter.BLL.Validators.EventNewsCategories;

namespace VictoryCenter.BLL.Validators.Localization.EventNewsCategories;

public class CreateEventNewsCategoryLocalizationValidator
    : AbstractValidator<CreateEventNewsCategoryLocalizationCommand>
{
    public CreateEventNewsCategoryLocalizationValidator()
    {
        RuleFor(command => command.Localization.EntityId).GreaterThan(0);
        RuleFor(command => command.Localization.LanguageId).GreaterThan(0);
        RuleFor(command => command.Localization.Name)
            .ValidEventNewsCategoryName();
    }
}
