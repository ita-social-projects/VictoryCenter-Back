using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNewsCategories.Update;
using VictoryCenter.BLL.Validators.EventNewsCategories;

namespace VictoryCenter.BLL.Validators.Localization.EventNewsCategories;

public class UpdateEventNewsCategoryLocalizationValidator
    : AbstractValidator<UpdateEventNewsCategoryLocalizationCommand>
{
    public UpdateEventNewsCategoryLocalizationValidator()
    {
        RuleFor(command => command.EntityId).GreaterThan(0);
        RuleFor(command => command.LanguageId).GreaterThan(0);
        RuleFor(command => command.Localization.Name)
            .ValidEventNewsCategoryName();
    }
}
