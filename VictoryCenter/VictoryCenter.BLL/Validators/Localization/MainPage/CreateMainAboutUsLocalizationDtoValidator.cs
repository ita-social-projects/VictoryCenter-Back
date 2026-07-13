using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

namespace VictoryCenter.BLL.Validators.Localization.MainPage;

public class CreateMainAboutUsLocalizationDtoValidator : AbstractValidator<CreateMainAboutUsLocalizationDto>
{
    public CreateMainAboutUsLocalizationDtoValidator(BaseMainPageLocalizationDtoValidator baseValidator)
    {
        RuleFor(x => x.EntityId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateMainAboutUsLocalizationDto.EntityId)));

        this.AddTitleAndDescriptionRules(MainPageConstants.Localization.ValidationSectionDescriptionRules.MaxLen);
    }
}
