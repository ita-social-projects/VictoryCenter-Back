using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.MainPage.Create;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.Localization.MainPage;

public class CreateMainPageLocalizationCommandValidator : AbstractValidator<CreateMainPageLocalizationCommand>
{
    public CreateMainPageLocalizationCommandValidator(CreateMainPageLocalizationDtoValidator dtoValidator)
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(CreateMainPageLocalizationCommand.Dto)))
            .SetValidator(dtoValidator!);
    }
}
