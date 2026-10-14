using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.EventNews.Create;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNews;
using VictoryCenter.BLL.Validators.Localization.Base;

namespace VictoryCenter.BLL.Validators.Localization.EventNews;

public class CreateEventNewsLocalizationValidator : AbstractValidator<CreateEventNewsLocalizationCommand>
{
    public CreateEventNewsLocalizationValidator(BaseEventNewsLocalizationValidator baseEventNewsLocalizationValidator)
    {
        RuleFor(x => x.CreateAdminEventNewsLocalizationDto)
            .SetValidator(new LocalizationIdentityValidator<CreateAdminEventNewsLocalizationDto>());

        RuleFor(c => c.CreateAdminEventNewsLocalizationDto).SetValidator(baseEventNewsLocalizationValidator);
    }
}
