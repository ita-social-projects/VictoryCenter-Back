using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Update;

namespace VictoryCenter.BLL.Validators.Localization.EventNews;

public class UpdateEventNewsLocalizationValidator : AbstractValidator<UpdateEventNewsLocalizationCommand>
{
    public UpdateEventNewsLocalizationValidator(BaseEventNewsLocalizationValidator baseEventNewsLocalizationValidator)
    {
        RuleFor(c => c.UpdateEventNewsLocalizationDto).SetValidator(baseEventNewsLocalizationValidator);
    }
}
