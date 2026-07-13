using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

namespace VictoryCenter.BLL.Validators.Localization.MainPage;

public class CreateMainPartnersLocalizationDtoValidator : AbstractValidator<CreateMainPartnersLocalizationDto>
{
    public CreateMainPartnersLocalizationDtoValidator(BaseMainPageLocalizationDtoValidator baseValidator)
    {
        RuleFor(x => x.EntityId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateMainPartnersLocalizationDto.EntityId)));

        this.AddTitleAndDescriptionRules(MainPageConstants.Localization.ValidationSectionDescriptionRules.MaxLen);
    }
}
