using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.MainPage.Create;

namespace VictoryCenter.BLL.Validators.Localization.MainPage;

public class CreateMainPageLocalizationCommandValidator : AbstractValidator<CreateMainPageLocalizationCommand>
{
    public CreateMainPageLocalizationCommandValidator(CreateMainPageLocalizationDtoValidator dtoValidator)
    {
        RuleFor(x => x.Dto).SetValidator(dtoValidator);
    }
}
