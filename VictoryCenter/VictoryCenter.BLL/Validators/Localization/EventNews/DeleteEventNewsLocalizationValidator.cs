using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Delete;

namespace VictoryCenter.BLL.Validators.Localization.EventNews;

public class DeleteEventNewsLocalizationValidator : AbstractValidator<DeleteEventNewsLocalizationCommand>
{
    public DeleteEventNewsLocalizationValidator()
    {
        RuleFor(x => x.EntityId).GreaterThan(0);
        RuleFor(x => x.LanguageId).GreaterThan(0);
    }
}
