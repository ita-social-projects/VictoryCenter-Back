using FluentValidation;
using VictoryCenter.BLL.Constants;
using VictoryCenter.BLL.DTOs.Admin.Localization.MainPage;

namespace VictoryCenter.BLL.Validators.Localization.MainPage;

public class CreateMainDonationsLocalizationDtoValidator : AbstractValidator<CreateMainDonationsLocalizationDto>
{
    public CreateMainDonationsLocalizationDtoValidator(BaseMainPageLocalizationDtoValidator baseValidator)
    {
        ArgumentNullException.ThrowIfNull(baseValidator);

        RuleFor(x => x.EntityId)
            .GreaterThan(0)
            .WithMessage(ErrorMessagesConstants.PropertyMustBePositive(nameof(CreateMainDonationsLocalizationDto.EntityId)));

        this.AddTitleAndDescriptionRules(MainPageConstants.Localization.SectionDescription.MaxLength);
    }
}
