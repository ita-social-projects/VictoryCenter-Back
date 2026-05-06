using FluentValidation;
using VictoryCenter.BLL.Commands.Admin.Localization.MainPage.Update;
using VictoryCenter.BLL.Constants;

namespace VictoryCenter.BLL.Validators.Localization.MainPage;

public class UpdateMainPageLocalizationCommandValidator : AbstractValidator<UpdateMainPageLocalizationCommand>
{
    public UpdateMainPageLocalizationCommandValidator(UpdateMainPageLocalizationDtoValidator dtoValidator)
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(ErrorMessagesConstants.PropertyIsRequired(nameof(UpdateMainPageLocalizationCommand.Dto)))
            .SetValidator(dtoValidator!);
    }
}
